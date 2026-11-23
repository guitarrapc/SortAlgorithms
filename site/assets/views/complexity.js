import { d3, PATTERNS, PATTERN_SHORT, PATTERN_HEAD, fmtT, fmtX, esc, svg, logTicks, showTip, hideTip, fitExponent, selectEl, SER, nudge, anchorBase, sourceUrl, intro, narrow, chartWidth, shortN, SCALING, nearestLine, pickRadius, chartDialog, EXPAND_ICON } from "../lib.js";

// Measured scaling exponent k (time ∝ n^k) per algorithm and input pattern, next to the complexity the
// algorithm's XML documentation states. Reference values depend on the size range, so they are fitted
// over the same sizes as the measurement.
const CLASSES = ["≈ n", "≈ n log n", "≈ n log² n", "n log² n – n²", "≈ n²"];
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
      ${intro(`How time grows with n: time ∝ nᵏ. At k = 1.00, time doubles when n doubles. At k = 2.00, it grows 4 times. k is not a speed: <a href="#leagues">Leagues</a> compares speed.`,
        "k is the slope of log(time) against log(n). Each cell has the color of the nearest reference in the legend. n log n and n log² n give different k for different size ranges, so each class row shows its own values.",
        "The last column is the documented best, average and worst case. A cell that reaches the documented worst case stands out. k also includes fixed costs, cache effects and branch prediction. Select an algorithm or a cell to see the slope of each size interval.")}
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
    // Each class gets its reference k for the sizes most classes share; a class with other sizes shows its own in its row.
    const big = run.allSizes.filter(n => n >= 256 && !run.sizesOf(SCALING).includes(n));
    const sz = mode === "fit" ? big : big.slice(-2), r = refs(sz);
    const ks = ["1.00", r.nl.toFixed(2), r.nl2.toFixed(2), "", "2.00"];
    root.querySelector("#k-l").innerHTML = `<span>k:</span>` + CLASSES.map((t, i) => `<span><span class="sq kb${i}"></span>${t}${ks[i] ? ` <b class="mono">${ks[i]}</b>` : ""}</span>`).join("") +
      `<span class="note">for n = ${shortN(sz[0])}–${shortN(sz.at(-1))}</span>`;
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
      (beyond.some(w => w.p !== "Random") ? `<div>Sorted, Reversed and PipeOrgan repeat one input in every iteration. At small sizes the branch predictor learns it, which can steepen the slope. StatisticsContext operation counts separate this from extra work.</div>` : "") +
      `<details class="more"><summary>≈ n² within the documented worst case: ${within.length} · lower class than on Random: ${lower}</summary>
        <div class="mchips"><span>Documented worst case n², or a bound in another variable such as m:</span>${chips(within)}</div>
        <p>${lower} cells grow slower than the same algorithm on Random (best-case behavior).</p></details>`;
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
        // Selecting the name keeps the selected pattern when this algorithm has it.
        const p0 = Number.isNaN(kOf(g.c, m, sel.p)) ? "Random" : sel.p;
        h += `<tr><td class="l${sel.c === g.c && sel.m === m ? " on" : ""}" tabindex="0" title="${esc(m)}" data-c="${esc(g.c)}" data-m="${esc(m)}" data-p="${p0}">${esc(m)}</td>`;
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
  // The line under the pointer comes forward and the others fade; a click selects its pattern, which anchors the guides.
  let hotP = null, big = null;
  const applyFocus = () => document.querySelectorAll(".plots.kx").forEach(el => {
    el.classList.toggle("focus", hotP != null);
    el.querySelectorAll(".ser").forEach(g => g.classList.toggle("on", g.dataset.id === hotP));
  });
  const setHot = p => { if (p !== hotP) { hotP = p; applyFocus(); } };
  const pickPattern = p => { if (p && p !== sel.p) select(sel.c, sel.m, p); };
  const legendHtml = () => PATTERNS.map((pp, i) => `<button type="button" class="lgb" data-p="${pp}" aria-pressed="${pp === sel.p}"><span class="sw" style="background:${SER(i)}"></span>${PATTERN_SHORT[pp]}</button>`).join("");
  // Legend entries and table rows act on the chart like its lines: pointing brings a pattern forward, clicking selects it.
  const bindPatterns = el => {
    el.addEventListener("pointerover", e => { if (e.pointerType !== "touch") setHot(e.target.closest("[data-p]")?.dataset.p ?? null); });
    el.addEventListener("pointerleave", () => setHot(null));
    el.addEventListener("click", e => pickPattern(e.target.closest("[data-p]")?.dataset.p));
  };

  const drawChart = (host, W, H) => {
    const { c, m, p } = sel, sz = run.sizesOf(c);
    const M = { l: W < 420 ? 52 : 60, r: 88, t: 12, b: 32 };
    const s = svg(host, W, H, `${m} scaling`);
    const series = PATTERNS.map((pp, i) => ({ id: pp, i, pts: sz.map(n => ({ n, t: run.get(c, m, n, pp)?.mean })).filter(q => q.t > 0) })).filter(z => z.pts.length);
    const all = series.flatMap(z => z.pts.map(q => q.t));
    const x = d3.scaleLog().base(2).domain([sz[0], sz.at(-1)]).range([M.l, W - M.r]);
    const y = d3.scaleLog().domain([d3.min(all) * 0.8, d3.max(all) * 1.25]).range([H - M.b, M.t]);
    const yt = logTicks(y, Math.max(7, Math.round(H / 45)));
    const clip = "k-clip-" + (host.closest("dialog") ? "big" : "page");
    s.append("defs").append("clipPath").attr("id", clip).append("rect").attr("x", M.l).attr("y", M.t).attr("width", W - M.l - M.r).attr("height", H - M.t - M.b);
    s.append("g").attr("class", "grid").selectAll("line").data(yt).join("line").attr("x1", M.l).attr("x2", W - M.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${H - M.b})`).call(d3.axisBottom(x).tickValues(sz).tickFormat(n => W < 420 ? shortN(n) : "n=" + n).tickSizeOuter(0));
    s.append("g").attr("class", "axis").attr("transform", `translate(${M.l},0)`).call(d3.axisLeft(y).tickValues(yt).tickFormat(fmtT).tickSize(0).tickPadding(6));
    const line = d3.line().x(q => x(q.n)).y(q => y(q.t));
    const labels = [];
    // The selected pattern is drawn last, above the others.
    for (const z of [...series].sort((a1, b1) => (a1.id === p) - (b1.id === p))) {
      const on = z.id === p, g = s.append("g").attr("class", "ser").attr("data-id", z.id);
      g.append("path").attr("class", "sl").attr("d", line(z.pts)).attr("stroke-width", on ? 2.8 : 1.6).style("fill", "none").style("stroke", SER(z.i));
      g.selectAll("circle").data(z.pts).join("circle").attr("cx", q => x(q.n)).attr("cy", q => y(q.t)).attr("r", on ? 3.8 : 2.6)
        .style("fill", SER(z.i)).style("stroke", "var(--panel)").style("stroke-width", 1.2);
      labels.push({ y: y(z.pts.at(-1).t), t: PATTERN_SHORT[z.id], on, g });
    }
    // The guides are dashed and drawn above the data, so a line that follows one still shows it.
    const start = (series.find(z => z.id === p) || series[0]).pts[0];
    const guides = s.append("g").attr("class", "guides").attr("clip-path", `url(#${clip})`);
    for (const [lab, fn] of [["n", n => n], ["n log n", n => n * Math.log2(n)], ["n²", n => n * n]]) {
      const dense = d3.range(0, 41).map(i => sz[0] * (sz.at(-1) / sz[0]) ** (i / 40)).map(n => ({ n, t: start.t * fn(n) / fn(sz[0]) }));
      guides.append("path").attr("d", line(dense));
      const end = dense.filter(q => q.t <= y.domain()[1]).at(-1);
      if (!end) continue;
      if (end.n < sz.at(-1) * 0.999) s.append("text").attr("x", x(end.n) + 4).attr("y", y(end.t) + 12).attr("class", "t-muted guide-l").text(lab);
      else labels.push({ y: y(end.t), t: lab, guide: true });
    }
    // Labels at the right edge are spread apart; a short leader joins a moved label to its line.
    for (const l of labels) l.y0 = l.y;
    nudge(labels, 12, M.t + 2, H - M.b);
    const lx = W - M.r + 16;
    for (const l of labels) {
      const into = l.g ?? s;
      if (Math.abs(l.y - l.y0) > 2) into.append("path").attr("class", "leader").attr("d", `M${W - M.r + 4},${l.y0}L${lx - 3},${l.y}`);
      into.append("text").attr("x", lx).attr("y", l.y + 4).attr("class", l.guide ? "t-muted guide-l" : "t-ink").style("font-weight", l.on ? 600 : null).style("font-size", "10.5px").text(l.t);
    }
    const X = q => x(q.n), Y = q => y(q.t);
    const near = ev => { const [px, py] = d3.pointer(ev, s.node()), { z, d } = nearestLine(series, px, py, X, Y); return d <= pickRadius(ev) ? z.id : null; };
    s.append("rect").attr("class", "hit").attr("x", M.l).attr("y", M.t).attr("width", W - M.l - M.r).attr("height", H - M.t - M.b).style("fill", "transparent")
      .on("pointermove pointerdown", ev => {
        const [px] = d3.pointer(ev, s.node()), n = d3.least(sz, v => Math.abs(x(v) - px)), id = near(ev);
        if (ev.pointerType !== "touch") setHot(id);
        const rows = series.map(z => ({ z, q: z.pts.find(q => q.n === n) })).filter(r => r.q).sort((a1, b1) => b1.q.t - a1.q.t);
        showTip(ev, `<b>${esc(m)} · n = ${n}</b>` + rows.map(({ z, q }) =>
          `<div class="tr${z.id === (id ?? p) ? " on" : ""}"><span class="sw" style="background:${SER(z.i)}"></span>${PATTERN_SHORT[z.id]} <b class="mono">${fmtT(q.t)}</b></div>`).join("") +
          `<span class="note">Click a line to select its pattern</span>`);
      })
      .on("click", ev => pickPattern(near(ev)))
      .on("pointerleave", ev => { if (ev.pointerType === "touch") return; hideTip(); setHot(null); });
    applyFocus();
  };

  // The chart at the size of the screen, with the pattern legend.
  const enlarge = () => {
    big = chartDialog(`${sel.m} · time against n`, (host, W, H) => {
      const wrap = document.createElement("div");
      wrap.className = "big-wrap";
      wrap.innerHTML = `<div class="legend">${legendHtml()}</div><div class="plots kx"></div>`;
      host.append(wrap);
      bindPatterns(wrap.querySelector(".legend"));
      drawChart(wrap.querySelector(".plots"), W, Math.max(220, H - wrap.firstChild.offsetHeight - 8));
    });
    big.dlg.addEventListener("close", () => { big = null; setHot(null); });
  };

  const drawDetail = () => {
    const { c, m, p } = sel, sz = run.sizesOf(c), a = algo(m);
    const host = root.querySelector("#k-d");
    const src = sourceUrl(a.file);
    const options = groups.map(g => `<optgroup label="${esc(g.label)}">${g.methods.map(mm =>
      `<option value="${esc(g.c + "|" + mm)}"${g.c === c && mm === m ? " selected" : ""}>${esc(mm)}</option>`).join("")}</optgroup>`).join("");
    host.innerHTML = `<div class="legend" style="gap:8px 12px"><select id="k-m" class="pick-m" aria-label="Algorithm">${options}</select><span class="cat">${esc(c)}</span>${src ? `<a href="${src}">source</a>` : ""}${m !== run.reference ? `<a href="#compare?${new URLSearchParams({ add: c + "|" + m, ...(params.run ? { run: params.run } : {}) })}">compare with others</a>` : ""}<span class="note">Pattern: ${esc(p)}. Dashed lines: n, n log n and n² growth from its smallest size.</span></div>
      <div class="legend" id="k-pl">${legendHtml()}<button type="button" class="btn xp" id="k-x">${EXPAND_ICON} Enlarge</button></div>
      <div class="twocol"><div id="k-g" class="plots kx"></div><div id="k-r"></div></div>`;
    const W = chartWidth(host.querySelector("#k-g"), 280, 620), H = Math.round(Math.max(240, Math.min(330, W * 0.62)));
    drawChart(host.querySelector("#k-g"), W, H);
    bindPatterns(host.querySelector("#k-pl"));
    host.querySelector("#k-x").addEventListener("click", enlarge);

    const ivs = d3.pairs(sz), maxN = sz.at(-1);
    let t = `<div class="docbox">${a.reference ? "<div>.NET MemoryExtensions.Sort, measured in every benchmark class as a shared reference.</div>" : a.best
      ? `<div><span>Best</span>${esc(a.best)}</div><div><span>Average</span>${esc(a.average)}</div><div><span>Worst</span>${esc(a.worst)}</div>` : "<div>No Performance Characteristics found in the documentation.</div>"}</div>`;
    t += `<div class="scroll" style="margin-top:10px"><table class="st"><thead><tr><th>Pattern</th><th>k, all sizes</th>${ivs.map(([u, v]) => `<th>${shortN(u)}→${shortN(v)}</th>`).join("")}<th>vs Random at n=${maxN}</th></tr></thead><tbody>`;
    PATTERNS.forEach((pp, i) => {
      const pts = sz.map(n => [n, run.get(c, m, n, pp)?.mean]);
      if (!pts.every(q => q[1] > 0)) return;
      const kAll = fitExponent(pts);
      t += `<tr data-p="${pp}"${pp === p ? ' class="on"' : ""}><td><span class="sw" style="background:${SER(i)}"></span>${PATTERN_SHORT[pp]}</td><td class="num"><span class="kchip kb${kclass(kAll, edgesOf(sz))}">${kAll.toFixed(2)}</span></td>` +
        ivs.map(([u, v]) => { const k = fitExponent([[u, run.get(c, m, u, pp).mean], [v, run.get(c, m, v, pp).mean]]); return `<td class="num"><span class="kchip kb${kclass(k, edgesOf([u, v]))}">${k.toFixed(2)}</span></td>`; }).join("") +
        `<td class="num">${fmtX(run.get(c, m, maxN, pp).mean / run.get(c, m, maxN, "Random").mean)}</td></tr>`;
    });
    host.querySelector("#k-r").innerHTML = t + `</tbody></table></div><p class="note" style="margin-top:6px">Each interval uses its own reference values. "vs Random" is the time ratio at the largest size.</p>`;
    bindPatterns(host.querySelector("#k-r tbody"));
  };

  // Scrolls the matrix box, not the page, so that the selected row is in view below the sticky header.
  // td.l is sticky, which makes its offsetParent the page, so the position is taken from the layout boxes.
  const showSelectedRow = () => {
    const cell = table.querySelector("td.sel, td.l.on");
    if (!cell) return;
    const box = table.getBoundingClientRect(), r = cell.getBoundingClientRect(), head = table.querySelector("thead")?.offsetHeight ?? 0;
    if (r.top < box.top + head || r.bottom > box.bottom) table.scrollTop += r.top - box.top - head - (table.clientHeight - head - r.height) / 2;
  };
  const select = (c, m, p) => {
    sel = { c, m, p };
    setParams({ c, m, p, mode: mode === "last" ? "last" : undefined });
    drawNotes(); drawMatrix(); drawDetail(); showSelectedRow();
    big?.redraw();
  };
  // The detail sits below the matrix: on phones it always scrolls into view, elsewhere only when it is off screen.
  const reveal = () => {
    const d = root.querySelector("#k-d");
    if (narrow() || d.getBoundingClientRect().top > innerHeight - 160) d.scrollIntoView({ behavior: "smooth", block: narrow() ? "start" : "nearest" });
  };
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
    reveal();
  });
  // The algorithm picker in the detail keeps the selected pattern when the new algorithm has it.
  root.querySelector("#k-d").addEventListener("change", e => {
    if (e.target.id !== "k-m") return;
    const [c, m] = e.target.value.split("|");
    const cf = root.querySelector("#k-c");
    if (cf.value !== "*" && cf.value !== c && m !== run.reference) { cf.value = c; setParams({ cat: c }); }
    select(c, m, run.get(c, m, run.maxSize(c) ?? 0, sel.p) ? sel.p : "Random");
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
