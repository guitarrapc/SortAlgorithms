import { d3, PATTERNS, PATTERN_SHORT, esc, svg, logTicks, showTip, hideTip, SER, anchorBase, anchorFactor, geo, fmtX, intro, foldFilters, chartWidth, seg, bindSeg, SCALING, homeOf, shortN, nearestLine, pickRadius, chartDialog, EXPAND_ICON, selectEl } from "../lib.js";

// Chosen algorithms side by side across sizes, one small chart per input pattern, in the form papers use:
// divided by n log₂ n, an n log n sort is flat, so where each algorithm sits reads directly.
// Each algorithm keeps the color of the slot it was added to, so removing one never repaints the others.
const MAX = 8;
const DEFAULTS = ["Insertion|InsertionSort", "Insertion|ShellSortCiura2001", "Network|BitonicSort", "Heap|HeapSort", "Merge|Driftsort", "Partition|PDQSort", "Distribution|RadixLSD256Sort"];
const f3 = d3.format(".3~r"), fTick = d3.format("~g");
const METRICS = {
  nlogn: { label: "÷ n log₂ n", axis: "time ÷ n log₂ n", f: (t, n) => t / (n * Math.log2(n)), fmt: v => f3(v) + " ns", tick: v => fTick(v) + " ns" },
  n: { label: "÷ n", axis: "time per element", f: (t, n) => t / n, fmt: v => f3(v) + " ns", tick: v => fTick(v) + " ns" },
  ref: { label: "÷ DotnetSort", axis: "time ÷ DotnetSort", f: (t, n, r) => t / r, fmt: fmtX, tick: v => "×" + fTick(v) },
};

export function render(root, { run, params, setParams, onResize }) {
  const base = anchorBase(run);
  const ref = run.reference;
  // An algorithm is chosen by its own class; the sizes the Scaling class adds extend the same line.
  const homes = run.categories.filter(c => c !== SCALING);
  const known = new Set(homes.flatMap(c => run.methodsOf(c).filter(m => m !== ref).map(m => c + "|" + m)));
  const factors = new Map(run.categories.map(c => [c, base ? anchorFactor(run, c, base) ?? 1 : 1]));
  let metric = METRICS[params.y] ? params.y : "nlogn";
  let shared = params.ys === "one";
  const slots = Array(MAX).fill(null);
  (params.a != null ? params.a.split(",") : DEFAULTS).slice(0, MAX).forEach((id, i) => { if (known.has(id) && !slots.includes(id)) slots[i] = id; });
  // Complexity links here with add=: the algorithm joins the current set, replacing the last slot when all are taken.
  const [addC, addM] = (params.add ?? "").split("|");
  const add = addC === SCALING ? homeOf(run, addM) + "|" + addM : params.add;
  if (known.has(add) && !slots.includes(add)) { const i = slots.indexOf(null); slots[i >= 0 ? i : MAX - 1] = add; }
  const large = run.sizesOf(SCALING), small = Math.max(...homes.map(c => run.maxSize(c) ?? 0));
  const sizeNote = large.length
    ? `Every class measures n = ${run.allSizes[0]} to ${small}. The ${SCALING} class measures the algorithms that are not quadratic on any pattern again, from n = ${large[0]} to ${large.at(-1)} (${large.at(-1) * 4 / 1048576} MB of int input); those points continue each line. ${SCALING} shares no size with the other classes, so its times are not drift-corrected.`
    : `The sizes are the ones the benchmark measures, n = ${run.allSizes[0]} to ${run.allSizes.at(-1)} int elements, which is at most ${run.allSizes.at(-1) * 4 / 1024} KB of input. Papers usually go on to millions of elements, where memory effects dominate; these charts cover only the small sizes.`;

  root.innerHTML = `
    <section class="panel">
      <h2>Compare</h2>
      ${intro("Chosen algorithms, one chart per input pattern. Divided by n log₂ n, an n log n sort stays flat, a linear one falls and a quadratic one rises.",
        `Pick up to eight algorithms. The dashed gray line is ${esc(ref)} (.NET Span.Sort). Both axes are logarithmic. With one y axis for every pattern, the charts can be compared with each other; with an axis per chart, close algorithms separate more. Times from the other benchmark classes are scaled by one drift factor per class, so that their ${esc(ref)} matches the one in ${esc(base ?? "Partition")}.`,
        "Point at a line or a chip to bring that algorithm forward; click or tap to keep it there. The button beside each pattern name opens that chart at the size of the screen. The table below the charts lists the same values for one pattern.",
        sizeNote)}
      <div class="filters">
        ${seg("c-y", "Scale", Object.entries(METRICS).map(([k, v]) => [k, v.label]), metric).replace('class="fl"', 'class="fl wide"')}
        <div id="c-addw"></div>
        <label for="c-q" data-sum="">Quick pick<select id="c-q"><option value="">Choose…</option><option value="mix">One per complexity class</option>${homes.filter(c => run.sizesOf(c).length >= 2).map(c => `<option value="${esc(c)}">${esc(c)}: ${MAX} fastest</option>`).join("")}</select></label>
        <label class="check"><input type="checkbox" id="c-ys"${shared ? " checked" : ""}> One y axis for every pattern</label>
      </div>
      <div class="cmp-chips" id="c-k"></div>
      <p class="note sm-hide" id="c-n"></p>
      <div class="cmp-grid plots" id="c-g"></div>
      <details class="more cmp-tw"><summary>Values as a table</summary>
        <div class="filters">${selectEl("c-tp", "Pattern", PATTERNS.map(p => [p, PATTERN_SHORT[p]]), "Random")}</div>
        <div class="scroll" id="c-t"></div>
      </details>
    </section>`;
  foldFilters(root.querySelector(".panel"));

  const parts = id => id.split("|");
  const save = () => {
    const last = slots.findLastIndex(Boolean);
    setParams({ a: last < 0 ? "-" : slots.slice(0, last + 1).map(s => s ?? "").join(","), y: metric === "nlogn" ? undefined : metric, ys: shared ? "one" : undefined, add: undefined });
  };

  // The line under the pointer (hot) wins over the one kept by a click (pinned).
  let hot = null, pinned = null, shown;
  const applyFocus = () => {
    const id = hot ?? pinned;
    if (id === shown) return;
    shown = id;
    document.querySelectorAll(".plots.cmp, #c-g").forEach(el => {
      el.classList.toggle("focus", id != null);
      el.querySelectorAll("svg.chart").forEach(s => {
        const hit = s.querySelector(".hit");
        // The focused line is drawn last, above the others; without focus the reference goes back underneath.
        [...s.querySelectorAll(".ser")].sort((a, b) => (a.dataset.id === id) - (b.dataset.id === id) || a.dataset.o - b.dataset.o).forEach(g => s.insertBefore(g, hit));
        s.querySelectorAll(".ser").forEach(g => g.classList.toggle("on", g.dataset.id === id));
      });
    });
    document.querySelectorAll(".chip[data-id] .pin").forEach(b => b.setAttribute("aria-pressed", String(b.parentElement.dataset.id === pinned)));
  };
  const setHot = id => { hot = id; applyFocus(); };
  const pin = id => { pinned = pinned === id ? null : id; shown = undefined; applyFocus(); };

  const chipsHtml = removable => slots.map((id, i) => {
    if (!id) return "";
    const [c, m] = parts(id);
    return `<span class="chip" data-id="${esc(id)}" style="--c:${SER(i)}"><button type="button" class="pin" aria-pressed="${id === pinned}"><i class="ln"></i><span class="nm">${esc(m)}</span><span class="cat">${esc(c)}</span></button>${removable ? `<button type="button" class="rm" data-i="${i}" aria-label="Remove ${esc(m)}">×</button>` : ""}</span>`;
  }).join("") + (base ? `<span class="chip ref" data-id="ref"><button type="button" class="pin" aria-pressed="${pinned === "ref"}"><i class="ln dash"></i><span class="nm">${esc(ref)}</span><span class="cat">reference</span></button></span>` : "");
  // Chips are the legend: pointing at one brings its line forward, clicking keeps it there, × removes the algorithm.
  const bindChips = el => {
    el.addEventListener("click", e => {
      const rm = e.target.closest(".rm");
      if (rm) { if (pinned === slots[+rm.dataset.i]) pinned = null; slots[+rm.dataset.i] = null; hot = null; update(); return; }
      const c = e.target.closest(".chip[data-id]");
      if (c) pin(c.dataset.id);
    });
    el.addEventListener("pointerover", e => { if (e.pointerType !== "touch") setHot(e.target.closest(".chip[data-id]")?.dataset.id ?? null); });
    el.addEventListener("pointerleave", () => setHot(null));
  };

  const drawControls = () => {
    const full = slots.every(Boolean);
    root.querySelector("#c-addw").outerHTML = `<label for="c-add" id="c-addw" data-sum="">Add algorithm<select id="c-add"${full ? " disabled" : ""}><option value="">${full ? `${MAX} chosen; remove one first` : "Choose…"}</option>${homes.map(c => {
      const ms = run.methodsOf(c).filter(m => m !== ref && !slots.includes(c + "|" + m));
      return ms.length ? `<optgroup label="${esc(c)}">${ms.map(m => `<option value="${esc(c + "|" + m)}">${esc(m)}</option>`).join("")}</optgroup>` : "";
    }).join("")}</select></label>`;
    root.querySelector("#c-k").innerHTML = chipsHtml(true);
  };

  // The data of every chart, rebuilt when the chosen algorithms, the scale or the axis mode change.
  let model = null;
  const build = () => {
    const M = METRICS[metric];
    // The reference of the class that measured size n: the base class up to its largest size, Scaling beyond.
    const refAt = (n, p) => run.get(base, ref, n, p)?.mean ?? (run.get(SCALING, ref, n, p)?.mean ?? NaN) * factors.get(SCALING);
    const value = (t, n, p) => { const r = refAt(n, p); return t > 0 && (metric !== "ref" || r > 0) ? M.f(t, n, r) : NaN; };
    const pointsOf = (c, m, p) => [
      ...run.sizesOf(c).map(n => [n, run.get(c, m, n, p)?.mean * factors.get(c)]),
      ...run.sizesOf(SCALING).filter(n => !run.sizesOf(c).includes(n)).map(n => [n, run.get(SCALING, m, n, p)?.mean * factors.get(SCALING)]),
    ].map(([n, t]) => ({ n, v: value(t, n, p) })).filter(q => q.v > 0);
    const chosen = slots.map((id, i) => id && { id, i, c: parts(id)[0], m: parts(id)[1] }).filter(Boolean);
    const all = new Map(PATTERNS.map(p => [p, [
      ...chosen.map(s => ({ ...s, pts: pointsOf(s.c, s.m, p) })),
      ...(base ? [{ id: "ref", ref: true, m: ref, pts: pointsOf(base, ref, p) }] : []),
    ].filter(z => z.pts.length)]));
    const values = [...all.values()].flatMap(ss => ss.flatMap(s => s.pts.map(q => q.v)));
    const sizes = [...new Set([...all.values()].flatMap(ss => ss.flatMap(s => s.pts.map(q => q.n))))].sort((a, b) => a - b);
    model = { M, all, values, sizes };
  };

  // One pattern's chart, W × H pixels, into host.
  const plot = (host, p, W, H) => {
    const { M, all, values, sizes } = model, ss = all.get(p);
    if (!ss.length) return;
    const pad = { l: metric === "ref" ? 46 : 54, r: 10, t: 8, b: 24 };
    const x = d3.scaleLog().base(2).domain(sizes.length > 1 ? d3.extent(sizes) : [sizes[0] / 2, sizes[0] * 2]).range([pad.l + 6, W - pad.r - 6]);
    const vs = shared ? values : ss.flatMap(z => z.pts.map(q => q.v));
    const y = d3.scaleLog().domain([d3.min(vs) / 1.2, d3.max(vs) * 1.2]).range([H - pad.b, pad.t]);
    const yt = logTicks(y, Math.max(8, Math.round(H / 40))), line = d3.line().x(q => x(q.n)).y(q => y(q.v));
    const s = svg(host, W, H, `${PATTERN_SHORT[p]}: ${M.axis} against n`);
    s.append("g").attr("class", "grid").selectAll("line").data(yt).join("line").attr("x1", pad.l).attr("x2", W - pad.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "axis").attr("transform", `translate(${pad.l},0)`).call(d3.axisLeft(y).tickValues(yt).tickFormat(M.tick).tickSize(0).tickPadding(6));
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${H - pad.b})`).call(d3.axisBottom(x).tickValues(sizes).tickFormat(shortN).tickSizeOuter(0));
    const guide = s.append("line").attr("y1", pad.t).attr("y2", H - pad.b).style("stroke", "var(--v-axis)").style("display", "none");
    // The reference is drawn first, under the chosen algorithms.
    [...ss].reverse().forEach((z, o) => {
      const g = s.append("g").attr("class", "ser").attr("data-id", z.id).attr("data-o", o);
      g.append("path").attr("class", "sl").attr("d", line(z.pts)).attr("stroke-width", z.ref ? 1.5 : 2).style("fill", "none").style("stroke", z.ref ? "var(--v-muted)" : SER(z.i)).style("stroke-dasharray", z.ref ? "4 3" : null);
      if (!z.ref) g.selectAll("circle").data(z.pts).join("circle").attr("cx", q => x(q.n)).attr("cy", q => y(q.v)).attr("r", 4).style("fill", SER(z.i)).style("stroke", "var(--panel)").style("stroke-width", 1.5);
    });
    const X = q => x(q.n), Y = q => y(q.v);
    const near = ev => { const [px, py] = d3.pointer(ev, s.node()), { z, d } = nearestLine(ss, px, py, X, Y); return d <= pickRadius(ev) ? z.id : null; };
    const hover = ev => {
      const [px] = d3.pointer(ev, s.node());
      const n = d3.least(sizes, v => Math.abs(x(v) - px)), id = near(ev);
      // A finger does not hover: a tap pins the line through the click instead.
      if (ev.pointerType !== "touch") setHot(id);
      const on = id ?? pinned;
      guide.attr("x1", x(n)).attr("x2", x(n)).style("display", null);
      const rows = ss.map(z => ({ z, q: z.pts.find(q => q.n === n) })).filter(r => r.q).sort((a, b) => a.q.v - b.q.v);
      showTip(ev, `<b>${PATTERN_SHORT[p]} · n = ${n}</b>` + rows.map(({ z, q }) =>
        `<div class="tr${z.id === on ? " on" : ""}"><i class="ln${z.ref ? " dash" : ""}" style="--c:${z.ref ? "var(--v-muted)" : SER(z.i)}"></i>${esc(z.m)} <b class="mono">${M.fmt(q.v)}</b></div>`).join(""));
    };
    s.append("rect").attr("class", "hit").attr("x", pad.l).attr("y", pad.t).attr("width", W - pad.l - pad.r).attr("height", H - pad.t - pad.b).style("fill", "transparent")
      .on("pointermove", hover).on("pointerdown", hover).on("click", ev => pin(near(ev) ?? pinned))
      .on("pointerleave", ev => { if (ev.pointerType === "touch") return; guide.style("display", "none"); hideTip(); setHot(null); });
  };

  // The values of one pattern's chart, for reading exact numbers and for readers who cannot use the charts.
  const drawTable = () => {
    const host = root.querySelector("#c-t"), p = root.querySelector("#c-tp").value;
    const { M, all, sizes } = model, ss = all.get(p) ?? [];
    if (!ss.length) { host.innerHTML = ""; return; }
    host.innerHTML = `<table class="st cmp-t"><caption class="sr-only">${esc(PATTERN_SHORT[p])}: ${esc(M.axis)}</caption><thead><tr><th>Algorithm</th>${sizes.map(n => `<th class="num">${shortN(n)}</th>`).join("")}</tr></thead><tbody>` +
      ss.map(z => `<tr><td><i class="ln${z.ref ? " dash" : ""}" style="--c:${z.ref ? "var(--v-muted)" : SER(z.i)}"></i>${esc(z.m)}</td>${sizes.map(n => { const q = z.pts.find(o => o.n === n); return `<td class="num">${q ? M.fmt(q.v) : "—"}</td>`; }).join("")}</tr>`).join("") +
      `</tbody></table><p class="note">${esc(M.axis)} · columns are n</p>`;
  };

  const drawCharts = () => {
    build();
    drawTable();
    const grid = root.querySelector("#c-g");
    root.querySelector("#c-n").textContent = `y: ${model.M.axis}, ${shared ? "one axis for all six charts" : "an axis per chart"} · x: n · both logarithmic`;
    if (!model.values.length) { grid.innerHTML = `<p class="empty">Add an algorithm to compare.</p>`; return; }
    grid.innerHTML = PATTERNS.map(p => `<figure class="cmp-cell"><figcaption>${PATTERN_SHORT[p]}<button type="button" class="xp" data-p="${p}" aria-label="Enlarge ${PATTERN_SHORT[p]}" title="Enlarge">${EXPAND_ICON}</button></figcaption><div data-p="${p}"></div></figure>`).join("");
    const W = chartWidth(grid.querySelector("[data-p]"), 240, 560), H = Math.round(Math.max(170, Math.min(240, W * 0.62)));
    for (const p of PATTERNS) plot(grid.querySelector(`div[data-p="${p}"]`), p, W, H);
    shown = undefined;
    applyFocus();
    big?.redraw();
  };

  // One pattern at the size of the screen, with the chips as its legend and a pattern switch.
  let big = null;
  const enlarge = p0 => {
    let p = p0;
    // Six buttons on wide screens; a phone has room for one select.
    const tools = `<div class="seg sm-hide" role="group" aria-label="Pattern">${PATTERNS.map(q => `<button type="button" data-v="${q}" aria-pressed="${q === p}">${PATTERN_SHORT[q]}</button>`).join("")}</div>` +
      `<select class="sm-only" aria-label="Pattern">${PATTERNS.map(q => `<option value="${q}"${q === p ? " selected" : ""}>${PATTERN_SHORT[q]}</option>`).join("")}</select>`;
    big = chartDialog(`Compare · ${model.M.axis}`, (host, W, H) => {
      host.innerHTML = `<div class="cmp-chips"></div><div class="plots cmp"></div>`;
      const chips = host.querySelector(".cmp-chips");
      chips.innerHTML = chipsHtml(false);
      bindChips(chips);
      plot(host.querySelector(".plots"), p, W, Math.max(200, H - chips.offsetHeight - 8));
      shown = undefined;
      applyFocus();
    }, tools);
    const show = v => {
      p = v;
      big.dlg.querySelectorAll(".big-tools .seg button").forEach(x => x.setAttribute("aria-pressed", String(x.dataset.v === v)));
      big.dlg.querySelector(".big-tools select").value = v;
      big.redraw();
    };
    big.dlg.querySelector(".big-tools .seg").addEventListener("click", e => { const b = e.target.closest("button"); if (b) show(b.dataset.v); });
    big.dlg.querySelector(".big-tools select").addEventListener("change", e => show(e.target.value));
    big.dlg.addEventListener("close", () => { big = null; hot = null; shown = undefined; applyFocus(); });
  };

  const update = () => {
    if (pinned && pinned !== "ref" && !slots.includes(pinned)) pinned = null;
    save(); drawControls(); drawCharts();
  };

  bindChips(root.querySelector("#c-k"));
  root.querySelector("#c-g").addEventListener("click", e => { const b = e.target.closest(".xp"); if (b) enlarge(b.dataset.p); });
  root.querySelector(".filters").addEventListener("change", e => {
    if (e.target.id === "c-ys") {
      shared = e.target.checked;
      save(); drawCharts();
    } else if (e.target.id === "c-add" && e.target.value) {
      slots[slots.indexOf(null)] = e.target.value;
      update();
    } else if (e.target.id === "c-q" && e.target.value) {
      const v = e.target.value;
      slots.fill(null);
      if (v === "mix") DEFAULTS.filter(id => known.has(id)).forEach((id, i) => { slots[i] = id; });
      else {
        // The fastest by the geometric mean of the six patterns at the category's largest size.
        const n = run.maxSize(v);
        run.methodsOf(v).filter(m => m !== ref)
          .map(m => ({ m, rows: PATTERNS.map(p => run.get(v, m, n, p)) })).filter(d => d.rows.every(Boolean))
          .map(d => ({ m: d.m, g: geo(d.rows.map(r => r.mean)) })).sort((a, b) => a.g - b.g)
          .slice(0, MAX).forEach((d, i) => { slots[i] = v + "|" + d.m; });
      }
      e.target.value = "";
      update();
    }
  });
  bindSeg(root, "c-y", v => { metric = v; save(); drawCharts(); });
  root.querySelector("#c-tp").addEventListener("change", drawTable);
  update();
  onResize(drawCharts);
}
