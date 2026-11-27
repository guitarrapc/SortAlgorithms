import { d3, PATTERNS, fmtT, fmtX, pct, esc, svg, showTip, hideTip, selectEl, seg, bindSeg, loadHistory, caseKey, machineOf, isaName, SER, fmtDate, intro, foldFilters, chartWidth } from "../lib.js";

// One case across all published runs. Runs on different machines (CPU model and instruction set) are not
// comparable, so lines only connect consecutive runs on the same machine and a vertical rule marks each change.
// Divided by the reference of the same run, a value mostly loses the clock difference between runners.
export async function render(root, { run, index, params, setParams, onResize }) {
  root.innerHTML = `<section class="panel"><h2>History</h2><p class="note">Loading history…</p></section>`;
  const hist = await loadHistory();
  const keys = Object.keys(hist.series);
  const parts = keys.map(k => k.split("|"));
  const cats = [...new Set(parts.map(p => p[0]))].sort();
  let [c, m, n, p] = (params.k && hist.series[params.k] ? params.k : (hist.series[caseKey("Partition", "PDQSort", 8192, "Random")] ? caseKey("Partition", "PDQSort", 8192, "Random") : keys[0])).split("|");
  const ref = run.reference;
  let byRef = params.y === "ref";
  const runs = index.runs;
  const machine = i => machineOf(runs[i]);
  const machines = [...new Set(runs.map(machineOf))];

  root.innerHTML = `
    <section class="panel">
      <h2>History</h2>
      ${intro("One case across every published run. Compare only points of the same color: each color is one CPU model and instruction set.",
        "Each point is the mean of one run, with ±StdDev as a band. A vertical rule marks a change of machine, and lines connect only consecutive runs on the same machine. Runners of one CPU model differ in clock and in AVX-512, so the instruction set is part of the machine. The table compares each run with the previous run on the same machine; \"within error\" means the 99.9% confidence intervals overlap.",
        `÷ ${esc(ref)} divides each run's time by that run's ${esc(ref)} in the same class, size, and pattern. A slower or faster runner then cancels out, so a change that remains comes from the code or from the instruction set.`)}
      <div class="filters" id="h-f"></div>
      <div class="filters">${seg("h-y", "Value", [["t", "Time"], ["ref", "÷ " + ref]], byRef ? "ref" : "t")}</div>
      <div class="legend after-sm">${machines.map((mdl, i) => `<span><span class="sw" style="background:${SER(i)}"></span>${esc(mdl)}</span>`).join("")}<span>vertical rule = machine changed</span></div>
      <div class="scroll" id="h-c"></div>
      <div class="tbox" id="h-t"></div>
    </section>`;
  foldFilters(root.querySelector(".panel"));

  const drawFilters = () => {
    const ms = [...new Set(parts.filter(q => q[0] === c).map(q => q[1]))].sort();
    if (!ms.includes(m)) m = ms[0];
    const ns = [...new Set(parts.filter(q => q[0] === c && q[1] === m).map(q => +q[2]))].sort((a, b) => a - b);
    if (!ns.includes(+n)) n = String(ns.at(-1));
    const ps = PATTERNS.filter(x => hist.series[caseKey(c, m, n, x)]);
    if (!ps.includes(p)) p = ps[0];
    root.querySelector("#h-f").innerHTML = selectEl("h-c1", "Category", cats, c) + selectEl("h-m", "Method", ms, m, "wide") + selectEl("h-n", "Size", ns.map(v => [v, "n = " + v]), n) + selectEl("h-p", "Pattern", ps, p);
  };

  // v is the plotted value, e its error and s its StdDev. A ratio carries the relative error of both runs' measurements.
  const points = key => {
    const refRows = new Map((hist.series[caseKey(c, ref, n, p)] ?? []).map(([i, mean, err, sd]) => [i, { mean, err, sd }]));
    return hist.series[key].flatMap(([i, mean, err, sd]) => {
      const base = { i, mean, run: runs[i], model: machine(i) };
      if (!byRef) return [{ ...base, v: mean, e: err, s: sd }];
      const r = refRows.get(i);
      if (!r || !(r.mean > 0)) return [];
      const v = mean / r.mean;
      return [{ ...base, v, e: err == null || r.err == null ? null : v * Math.hypot(err / mean, r.err / r.mean), s: v * Math.hypot(sd / mean, r.sd / r.mean) }];
    });
  };
  const fmtV = v => byRef ? fmtX(v) : fmtT(v);

  const drawChart = () => {
    const key = caseKey(c, m, n, p);
    setParams({ k: key, y: byRef ? "ref" : undefined });
    const pts = points(key);
    const host = root.querySelector("#h-c");
    host.innerHTML = "";
    if (!pts.length) {
      host.innerHTML = `<p class="empty">No run measured ${esc(ref)} together with this case.</p>`;
      root.querySelector("#h-t").innerHTML = "";
      return;
    }
    const W = chartWidth(host, 280, 1000), M = { l: W < 560 ? 54 : 66, r: 16, t: 0, b: 52 };
    const x = d3.scalePoint().domain(runs.map((_, i) => i)).range([M.l + 10, W - M.r - 10]);
    // Machine changes are labeled left to right, each in the highest row where it overlaps no other label, its rule
    // crosses no label below, and no earlier rule crosses it. A label goes right of its rule, or left where it fits better.
    // A change with no free row keeps its rule without a label; the point colors still name the machine.
    const ROW = 12, ROWS = 4, changes = [];
    runs.forEach((r, i) => {
      if (i === 0 || machine(i) === machine(i - 1)) return;
      const xx = (x(i) + x(i - 1)) / 2, text = "→ " + machine(i), tw = text.length * 5.8;
      const clash = (x0, x1, row) => changes.some(l => l.row >= 0 && ((l.row === row && l.x0 < x1 + 6 && x0 < l.x1 + 6) || (l.row > row && l.x0 <= xx && xx <= l.x1) || (l.row <= row && x0 <= l.xx && l.xx <= x1)));
      const spots = [xx + 4, xx - 4 - tw].filter(x0 => x0 >= 2 && x0 + tw <= W - 2).map(x0 => {
        let row = 0;
        while (row < ROWS && clash(x0, x0 + tw, row)) row++;
        return { x0, row };
      });
      const best = d3.least(spots.filter(s => s.row < ROWS), s => s.row);
      changes.push(best ? { xx, x0: best.x0, x1: best.x0 + tw, row: best.row, text } : { xx, row: -1 });
    });
    const labelRows = changes.length ? d3.max(changes, l => l.row) + 1 : 0;
    M.t = 16 + labelRows * ROW;
    const H = Math.round(Math.max(240, Math.min(340, W * 0.5))) + labelRows * ROW;
    const s = svg(host, W, H, `History of ${key}${byRef ? " ÷ " + ref : ""}`);
    const lo = d3.min(pts, q => q.v - q.s), hi = d3.max(pts, q => q.v + q.s);
    const y = d3.scaleLinear().domain([lo - (hi - lo) * 0.1, hi + (hi - lo) * 0.1 || hi * 1.05]).nice().range([H - M.b, M.t]);
    s.append("g").attr("class", "grid").selectAll("line").data(y.ticks(5)).join("line").attr("x1", M.l).attr("x2", W - M.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "axis").attr("transform", `translate(${M.l},0)`).call(d3.axisLeft(y).ticks(5).tickFormat(byRef ? v => "×" + d3.format("~r")(v) : fmtT).tickSize(0).tickPadding(6));
    // Date ticks: every step-th run and the last one, without a tick crowding the last and without repeating a day.
    const step = Math.ceil(runs.length / Math.max(4, Math.floor((W - M.l - M.r) / 36)));
    const day = i => runs[i].timestamp.slice(5, 10);
    let ticks = runs.map((_, i) => i).filter(i => i % step === 0 || i === runs.length - 1);
    if (ticks.length > 1 && ticks.at(-1) - ticks.at(-2) < step) ticks.splice(-2, 1);
    ticks = ticks.filter((i, k) => k === 0 || day(i) !== day(ticks[k - 1]));
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${H - M.b})`)
      .call(d3.axisBottom(x).tickValues(ticks).tickFormat(day).tickSizeOuter(0))
      .selectAll("text").attr("transform", "rotate(-35)").attr("text-anchor", "end").attr("dx", "-4").attr("dy", "6");
    changes.forEach(l => {
      const ty = 12 + l.row * ROW;
      s.append("line").attr("x1", l.xx).attr("x2", l.xx).attr("y1", l.text ? ty - 8 : M.t).attr("y2", H - M.b).style("stroke", "var(--v-ink-2)");
      if (l.text) s.append("text").attr("x", l.x0).attr("y", ty).attr("class", "t-muted").style("font-size", "10px").text(l.text);
    });
    let line = [];
    const flush = () => { if (line.length > 1) s.append("path").attr("d", d3.line().x(q => x(q.i)).y(q => y(q.v))(line)).style("fill", "none").style("stroke", SER(machines.indexOf(line[0].model))).style("stroke-width", 2); line = []; };
    pts.forEach(q => {
      if (line.length && (line[0].model !== q.model || runs.slice(line.at(-1).i + 1, q.i).some(r => machineOf(r) !== q.model))) flush();
      line.push(q);
    });
    flush();
    const tipOf = q => `<b>${esc(fmtDate(q.run.timestamp))}</b><br>${esc(q.model)} · ${esc(q.run.cpu)}<br>${byRef ? `÷ ${esc(ref)} <b class="mono">${fmtX(q.v)}</b> (${fmtT(q.mean)})` : `Mean <b class="mono">${fmtT(q.v)}</b> ± SD ${fmtT(q.s)}`}${q.run.commit ? `<br><span class="mono">${esc(q.run.commit.slice(0, 7))}</span>` : ""}`;
    pts.forEach(q => {
      const col = SER(machines.indexOf(q.model));
      s.append("line").attr("x1", x(q.i)).attr("x2", x(q.i)).attr("y1", y(q.v - q.s)).attr("y2", y(q.v + q.s)).style("stroke", col).style("stroke-width", 6).style("stroke-opacity", 0.25).style("stroke-linecap", "round");
      s.append("circle").attr("cx", x(q.i)).attr("cy", y(q.v)).attr("r", 4.5).style("fill", col).style("stroke", "var(--panel)").style("stroke-width", 2);
    });
    const ring = s.append("circle").attr("r", 8).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5).style("display", "none");
    const hover = ev => {
      const [px] = d3.pointer(ev, s.node());
      const q = d3.least(pts, o => Math.abs(x(o.i) - px));
      ring.attr("cx", x(q.i)).attr("cy", y(q.v)).style("display", null);
      showTip(ev, tipOf(q));
    };
    s.append("rect").attr("x", M.l).attr("y", M.t).attr("width", W - M.l - M.r).attr("height", H - M.t - M.b).style("fill", "transparent")
      .on("pointermove", hover).on("pointerdown", hover).on("pointerleave", () => { ring.style("display", "none"); hideTip(); });

    let t = `<table class="st"><thead><tr><th>Run</th><th>Machine</th><th>${byRef ? "÷ " + esc(ref) : "Mean"}</th><th class="sm-hide">Error</th><th>CV</th><th>Change<span class="md-hide"> vs previous run, same machine</span></th><th class="sm-hide">Source</th></tr></thead><tbody>`;
    [...pts].reverse().forEach(q => {
      const prev = pts.filter(o => o.i < q.i && o.model === q.model).at(-1);
      let change = "—";
      if (prev) {
        const d = q.v / prev.v - 1;
        const sig = prev.e != null && q.e != null ? (q.v - q.e > prev.v + prev.e ? "slower" : q.v + q.e < prev.v - prev.e ? "faster" : "within error") : "";
        change = `${d >= 0 ? "+" : ""}${(d * 100).toFixed(1)}%${sig ? ` <span class="sig">${sig}</span>` : ""}`;
      }
      const link = q.run.runUrl ? `<a href="${esc(q.run.runUrl)}">${q.run.source === "readme" ? "README import" : "workflow run"}</a>` : esc(q.run.source);
      t += `<tr${q.run.id === run.entry.id ? ' class="on"' : ""}><td class="wrap-sm"><span class="md-hide">${esc(fmtDate(q.run.timestamp))}</span><span class="md-only">${esc(fmtDate(q.run.timestamp).slice(5, 16))}</span></td><td class="wrap-sm" title="${esc(q.run.cpu)}"><span class="md-hide">${esc(q.run.cpu)}${q.run.isa ? " · " + esc(isaName(q.run.isa)) : ""}</span><span class="md-only">${esc(q.model)}</span></td><td class="num">${fmtV(q.v)}</td><td class="num sm-hide">${q.e == null ? "—" : "±" + fmtV(q.e).replace("×", "")}</td><td class="num">${pct(q.s / q.v)}</td><td class="num chg">${change}</td><td class="sm-hide">${link}</td></tr>`;
    });
    root.querySelector("#h-t").innerHTML = t + "</tbody></table>";
  };

  root.querySelector("#h-f").addEventListener("input", e => {
    const id = e.target.id, v = e.target.value;
    if (id === "h-c1") c = v; else if (id === "h-m") m = v; else if (id === "h-n") n = v; else if (id === "h-p") p = v;
    drawFilters(); drawChart();
  });
  bindSeg(root, "h-y", v => { byRef = v === "ref"; drawChart(); });
  drawFilters();
  drawChart();
  onResize(drawChart);
}
