import { d3, PATTERNS, PATTERN_SHORT, PATTERN_HEAD, fmtT, fmtX, esc, svg, logTicks, showTip, hideTip, fitExponent, selectEl, SER, nudge, anchorBase, sourceUrl, intro, narrow, chartWidth } from "../lib.js";

// Measured scaling exponent k (time ∝ n^k) per algorithm and input pattern, next to the complexity the
// algorithm's XML documentation states. Reference values depend on the size range, so they are fitted
// over the same sizes as the measurement.
const CLASSES = ["≈ n", "≈ n log n", "≈ n log² n", "n log² n – n²", "≈ n²"];
const shortN = n => n >= 1024 ? n / 1024 + "k" : String(n);
const shortC = s => s ? s.split(" - ")[0].split(" (")[0].trim() : "—";
const refs = sz => ({ nl: fitExponent(sz.map(n => [n, n * Math.log2(n)])), nl2: fitExponent(sz.map(n => [n, n * Math.log2(n) ** 2])) });
const edgesOf = sz => { const r = refs(sz); return [(1 + r.nl) / 2, (r.nl + r.nl2) / 2, r.nl2 + 0.15, 1.8]; };
const kclass = (k, e) => { let i = 0; while (i < e.length && k > e[i]) i++; return i; };

export function render(root, { run, params, setParams, onResize }) {
  let mode = params.mode === "last" ? "last" : "fit";
  const base = anchorBase(run);
  const groups = [...run.categories.filter(c => run.sizesOf(c).length >= 2).map(c => ({ c, label: c, methods: run.methodsOf(c).filter(m => m !== run.reference) })),
    ...(base ? [{ c: base, label: "Reference", methods: [run.reference] }] : [])];
  if (!groups.length) {
    root.innerHTML = `<section class="panel empty">This run has no category measured at two or more sizes.</section>`;
    return;
  }
  const first = groups.find(g => g.c === "Partition") || groups[0];
  let sel = { c: params.c || first?.c, m: params.m || (first?.methods.includes("QuickSort") ? "QuickSort" : first?.methods[0]), p: PATTERNS.includes(params.p) ? params.p : "PipeOrgan" };
  if (!run.get(sel.c, sel.m, run.maxSize(sel.c) ?? 0, sel.p)) sel = { c: first.c, m: first.methods[0], p: "Random" };

  root.innerHTML = `
    <section class="panel">
      <h2>Complexity</h2>
      ${intro("How each algorithm's time grows with n. Select a cell to compare it with the documented complexity.",
        "k is the slope of log(time) against log(n). Cells are colored by the nearest reference: n is 1.00 and n² is 2.00 at any size range, while n log n and n log² n depend on the sizes used.",
        "The documented Best / Average / Worst cases come from each algorithm's XML documentation, so a pattern that reaches the documented worst case stands out. k also includes fixed costs, cache effects and branch prediction; the detail shows the slope of each size interval.")}
      <div class="filters">
        ${selectEl("k-c", "Category", [["*", "All categories"], ...groups.filter(g => g.label !== "Reference").map(g => [g.c, g.c])], params.cat || "*", "wide")}
        <div class="fl wide"><span>Fit</span><div class="seg" role="group" id="k-mode"><button type="button" data-v="fit" aria-pressed="${mode === "fit"}">All sizes</button><button type="button" data-v="last" aria-pressed="${mode === "last"}">Last interval only</button></div></div>
      </div>
      <div class="legend" id="k-l"></div>
      <div id="k-n" class="note"></div>
      <div class="vscroll" id="k-t" style="max-height:min(460px, 62vh)"></div>
    </section>
    <section class="panel" id="k-d"></section>`;

  const sizesFor = c => mode === "fit" ? run.sizesOf(c) : run.sizesOf(c).slice(-2);
  const kOf = (c, m, p) => {
    const pts = sizesFor(c).map(n => [n, run.get(c, m, n, p)?.mean]);
    return pts.every(q => q[1] > 0) ? fitExponent(pts) : NaN;
  };
  const algo = m => run.algo(m) || {};
  const avgQuadratic = m => /²/.test(shortC(algo(m).average));
  // Whether the documented worst case allows time ∝ n²: true or false, or null when the bound names
  // another variable (d, k, m) or cannot be read. Θ(n·m) allows n² through m = n, so it is not "below n²".
  const worstAllowsN2 = m => {
    const terms = [...shortC(algo(m).worst).toLowerCase().matchAll(/[θoω]\(((?:[^()]|\([^()]*\))*)\)/g)].map(t => t[1].replace(/\s+/g, ""));
    if (!terms.length || terms.some(t => /[a-z]/.test(t.replace(/log[₂₃]?/g, "").replace(/n/g, "")))) return null;
    return terms.some(t => /n²|n³/.test(t) || /n\^\{?\d+\}?(?!\/)/.test(t));
  };
  const table = root.querySelector("#k-t");

  const drawLegend = () => {
    const big = run.allSizes.filter(n => n >= 256);
    const r4 = refs(mode === "fit" ? big : big.slice(-2)), r2 = refs([256, 1024]);
    root.querySelector("#k-l").innerHTML = `<span>k (time ∝ nᵏ)</span>` + CLASSES.map((t, i) => `<span><span class="sq kb${i}"></span>${t}</span>`).join("") +
      `<span class="note sm-hide">n log n / n log² n reference: ${r4.nl.toFixed(2)} / ${r4.nl2.toFixed(2)} for ${shortN(big[mode === "fit" ? 0 : big.length - 2])}–${shortN(big.at(-1))}, ${r2.nl.toFixed(2)} / ${r2.nl2.toFixed(2)} for 256–1k</span>`;
  };
  const drawNotes = () => {
    const beyond = [], within = [];
    let lower = 0;
    for (const g of groups) {
      if (g.label === "Reference") continue;
      const e = edgesOf(sizesFor(g.c));
      for (const m of g.methods) {
        const rnd = kclass(kOf(g.c, m, "Random"), e);
        for (const p of PATTERNS) {
          const k = kOf(g.c, m, p);
          if (Number.isNaN(k)) continue;
          const b = kclass(k, e);
          if (b === 4 && !avgQuadratic(m)) (worstAllowsN2(m) === false ? beyond : within).push({ c: g.c, m, p });
          if (p !== "Random" && b < rnd) lower++;
        }
      }
    }
    const chips = list => list.length ? list.map(w =>
      `<button type="button" data-c="${esc(w.c)}" data-m="${esc(w.m)}" data-p="${w.p}" aria-pressed="${w.c === sel.c && w.m === sel.m && w.p === sel.p}">${esc(w.m)} · ${PATTERN_SHORT[w.p]}</button>`).join("") : "<span>none</span>";
    root.querySelector("#k-n").innerHTML =
      `<div class="mchips"><span>≈ n² although the documented worst case is below n²:</span>${chips(beyond)}</div>` +
      (beyond.some(w => w.p !== "Random") ? `<div>Sorted, Reversed and PipeOrgan give every iteration the same input, so the branch predictor can learn it at small sizes and steepen the slope. StatisticsContext operation counts tell that apart from extra work.</div>` : "") +
      `<details class="more"><summary>Reaches ≈ n² within its documented worst case: ${within.length} · faster class than on Random: ${lower}</summary>
        <div class="mchips"><span>Within a documented worst case of n², or of a bound in another variable such as m:</span>${chips(within)}</div>
        <p>${lower} cells have a lower class than the same algorithm on Random (best-case behavior).</p></details>`;
  };
  const drawMatrix = () => {
    const cf = root.querySelector("#k-c").value;
    let h = `<table class="hm"><thead><tr><th></th>${PATTERNS.map(p => `<th>${PATTERN_HEAD[p]}</th>`).join("")}<th class="doc" style="text-align:left">Documented best / average / worst</th></tr></thead><tbody>`;
    for (const g of groups) {
      if (cf !== "*" && g.c !== cf && g.label !== "Reference") continue;
      const sz = sizesFor(g.c), r = refs(sz), e = edgesOf(sz);
      h += `<tr><td class="g" colspan="8"><span>${esc(g.label)}</span><span class="gs">${sz.map(shortN).join("→")} · n log n = ${r.nl.toFixed(2)}, n log² n = ${r.nl2.toFixed(2)}</span></td></tr>`;
      for (const m of g.methods) {
        const a = algo(m);
        h += `<tr><td class="l" title="${esc(m)}">${esc(m)}</td>`;
        for (const p of PATTERNS) {
          const k = kOf(g.c, m, p);
          if (Number.isNaN(k)) { h += `<td class="c">—</td>`; continue; }
          const on = sel.c === g.c && sel.m === m && sel.p === p;
          h += `<td class="c kb${kclass(k, e)}${on ? " sel" : ""}" tabindex="0" data-c="${esc(g.c)}" data-m="${esc(m)}" data-p="${p}" data-k="${k}">${k.toFixed(2)}</td>`;
        }
        h += `<td class="doc">${a.reference ? ".NET Span.Sort (reference)" : a.best ? `${esc(shortC(a.best))} / ${esc(shortC(a.average))} / ${esc(shortC(a.worst))}` : "—"}</td></tr>`;
      }
    }
    table.innerHTML = h + "</tbody></table>";
  };
  const drawDetail = () => {
    const { c, m, p } = sel, sz = run.sizesOf(c), a = algo(m);
    const host = root.querySelector("#k-d");
    const src = sourceUrl(a.file);
    host.innerHTML = `<div class="legend" style="gap:8px 12px"><h3>${esc(m)}</h3><span class="cat">${esc(c)}</span>${src ? `<a href="${src}">source</a>` : ""}<span class="note">selected pattern: ${esc(p)}. Thin gray lines: n, n log n and n² growth from the selected pattern's smallest size.</span></div>
      <div class="legend">${PATTERNS.map((pp, i) => `<span><span class="sw" style="background:${SER(i)}"></span>${PATTERN_SHORT[pp]}</span>`).join("")}</div>
      <div class="twocol"><div id="k-g"></div><div id="k-r"></div></div>`;
    const W = chartWidth(host.querySelector("#k-g"), 280, 620), H = Math.round(Math.max(240, Math.min(330, W * 0.62)));
    const M = { l: W < 420 ? 52 : 60, r: 74, t: 12, b: 32 };
    const s = svg(host.querySelector("#k-g"), W, H, `${m} scaling`);
    const series = PATTERNS.map((pp, i) => ({ p: pp, i, pts: sz.map(n => ({ n, t: run.get(c, m, n, pp)?.mean })).filter(q => q.t > 0) })).filter(z => z.pts.length);
    const all = series.flatMap(z => z.pts.map(q => q.t));
    const x = d3.scaleLog().base(2).domain([sz[0], sz.at(-1)]).range([M.l, W - M.r]);
    const y = d3.scaleLog().domain([d3.min(all) * 0.8, d3.max(all) * 1.25]).range([H - M.b, M.t]);
    s.append("defs").append("clipPath").attr("id", "k-clip").append("rect").attr("x", M.l).attr("y", M.t).attr("width", W - M.l - M.r).attr("height", H - M.t - M.b);
    s.append("g").attr("class", "grid").selectAll("line").data(logTicks(y)).join("line").attr("x1", M.l).attr("x2", W - M.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${H - M.b})`).call(d3.axisBottom(x).tickValues(sz).tickFormat(n => W < 420 ? shortN(n) : "n=" + n).tickSizeOuter(0));
    s.append("g").attr("class", "axis").attr("transform", `translate(${M.l},0)`).call(d3.axisLeft(y).tickValues(logTicks(y)).tickFormat(fmtT).tickSize(0).tickPadding(6));
    const line = d3.line().x(q => x(q.n)).y(q => y(q.t));
    const start = (series.find(z => z.p === p) || series[0]).pts[0];
    const guides = s.append("g").attr("clip-path", "url(#k-clip)");
    for (const [lab, fn] of [["n", n => n], ["n log n", n => n * Math.log2(n)], ["n²", n => n * n]]) {
      const dense = d3.range(0, 41).map(i => sz[0] * (sz.at(-1) / sz[0]) ** (i / 40)).map(n => ({ n, t: start.t * fn(n) / fn(sz[0]) }));
      guides.append("path").attr("d", line(dense)).style("fill", "none").style("stroke", "var(--v-muted)").style("stroke-width", 1);
      const end = dense.filter(q => q.t <= y.domain()[1]).at(-1);
      if (end) s.append("text").attr("x", x(end.n) + 4).attr("y", y(end.t) + (end.n < sz.at(-1) ? 12 : 4)).attr("class", "t-muted").style("font-size", "10px").text(lab);
    }
    const labels = [];
    for (const z of [...series].sort((a1, b1) => (a1.p === p) - (b1.p === p))) {
      const on = z.p === p;
      s.append("path").attr("d", line(z.pts)).style("fill", "none").style("stroke", SER(z.i)).style("stroke-width", on ? 2.8 : 1.5);
      s.selectAll(null).data(z.pts).join("circle").attr("cx", q => x(q.n)).attr("cy", q => y(q.t)).attr("r", on ? 3.8 : 2.6)
        .style("fill", SER(z.i)).style("stroke", "var(--panel)").style("stroke-width", 1.2)
        .on("pointermove", (ev, q) => showTip(ev, `<b>${esc(m)}</b> · ${esc(z.p)}<br><span class="mono">n=${q.n} ${fmtT(q.t)}</span>`)).on("pointerleave", hideTip);
      labels.push({ y: y(z.pts.at(-1).t), t: PATTERN_SHORT[z.p], on });
    }
    nudge(labels).forEach(l => s.append("text").attr("x", W - M.r + 8).attr("y", l.y + 4).attr("class", "t-ink").style("font-weight", l.on ? 600 : 400).style("font-size", "10.5px").text(l.t));

    const ivs = d3.pairs(sz), maxN = sz.at(-1);
    let t = `<div class="docbox">${a.reference ? "<div>.NET MemoryExtensions.Sort, measured in every benchmark class as a shared reference.</div>" : a.best
      ? `<div><span>Best</span>${esc(a.best)}</div><div><span>Average</span>${esc(a.average)}</div><div><span>Worst</span>${esc(a.worst)}</div>` : "<div>No Performance Characteristics found in the documentation.</div>"}</div>`;
    t += `<div class="scroll" style="margin-top:10px"><table class="st"><thead><tr><th>Pattern</th><th>k, all sizes</th>${ivs.map(([u, v]) => `<th>${shortN(u)}→${shortN(v)}</th>`).join("")}<th>vs Random at n=${maxN}</th></tr></thead><tbody>`;
    PATTERNS.forEach((pp, i) => {
      const pts = sz.map(n => [n, run.get(c, m, n, pp)?.mean]);
      if (!pts.every(q => q[1] > 0)) return;
      const kAll = fitExponent(pts);
      t += `<tr${pp === p ? ' class="on"' : ""}><td><span class="sw" style="background:${SER(i)}"></span>${PATTERN_SHORT[pp]}</td><td class="num"><span class="kchip kb${kclass(kAll, edgesOf(sz))}">${kAll.toFixed(2)}</span></td>` +
        ivs.map(([u, v]) => { const k = fitExponent([[u, run.get(c, m, u, pp).mean], [v, run.get(c, m, v, pp).mean]]); return `<td class="num"><span class="kchip kb${kclass(k, edgesOf([u, v]))}">${k.toFixed(2)}</span></td>`; }).join("") +
        `<td class="num">${fmtX(run.get(c, m, maxN, pp).mean / run.get(c, m, maxN, "Random").mean)}</td></tr>`;
    });
    host.querySelector("#k-r").innerHTML = t + `</tbody></table></div><p class="note" style="margin-top:6px">Each interval is classified against the reference values of that interval. "vs Random" compares the constant factor at the largest size.</p>`;
  };

  const select = (c, m, p) => { sel = { c, m, p }; setParams({ c, m, p, mode: mode === "last" ? "last" : undefined }); drawNotes(); drawMatrix(); drawDetail(); };
  const reveal = () => { if (narrow()) root.querySelector("#k-d").scrollIntoView({ behavior: "smooth", block: "start" }); };
  const pick = el => { const td = el.closest("[data-m]"); if (td) { select(td.dataset.c, td.dataset.m, td.dataset.p); reveal(); } };
  table.addEventListener("click", e => pick(e.target));
  table.addEventListener("keydown", e => { if (e.key === "Enter" || e.key === " ") { e.preventDefault(); pick(e.target); } });
  table.addEventListener("pointermove", ev => {
    if (ev.pointerType === "touch") return;
    const td = ev.target.closest("td.c[data-m]");
    if (!td) return hideTip();
    const k = +td.dataset.k, a = algo(td.dataset.m);
    showTip(ev, `<b>${esc(td.dataset.m)}</b> · ${esc(td.dataset.p)}<br>k = <b class="mono">${k.toFixed(2)}</b> (${CLASSES[kclass(k, edgesOf(sizesFor(td.dataset.c)))]})${a.average ? `<br><span class="mono">Avg ${esc(shortC(a.average))} · Worst ${esc(shortC(a.worst))}</span>` : ""}<br><span class="note">Click for details</span>`);
  });
  table.addEventListener("pointerleave", hideTip);
  root.querySelector("#k-n").addEventListener("click", e => {
    const b = e.target.closest("button[data-m]");
    if (!b) return;
    root.querySelector("#k-c").value = "*";
    select(b.dataset.c, b.dataset.m, b.dataset.p);
    const cell = table.querySelector("td.sel");
    if (cell) table.scrollTop = cell.offsetTop - 60;
    reveal();
  });
  root.querySelector("#k-c").addEventListener("input", e => { setParams({ cat: e.target.value === "*" ? undefined : e.target.value }); drawMatrix(); });
  root.querySelector("#k-mode").addEventListener("click", e => {
    const b = e.target.closest("button");
    if (!b) return;
    root.querySelectorAll("#k-mode button").forEach(x => x.setAttribute("aria-pressed", String(x === b)));
    mode = b.dataset.v; drawLegend(); select(sel.c, sel.m, sel.p);
  });
  drawLegend();
  select(sel.c, sel.m, sel.p);
  onResize(drawDetail);
}
