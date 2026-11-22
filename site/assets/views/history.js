import { d3, PATTERNS, fmtT, pct, esc, svg, showTip, hideTip, selectEl, loadHistory, caseKey, cpuModel, SER, fmtDate, intro, chartWidth } from "../lib.js";

// One case across all published runs. Runs on different CPU models are not comparable, so lines only
// connect consecutive runs on the same model and a vertical rule marks each change of model.
export async function render(root, { run, index, params, setParams, onResize }) {
  root.innerHTML = `<section class="panel"><h2>History</h2><p class="note">Loading history…</p></section>`;
  const hist = await loadHistory();
  const keys = Object.keys(hist.series);
  const parts = keys.map(k => k.split("|"));
  const cats = [...new Set(parts.map(p => p[0]))].sort();
  let [c, m, n, p] = (params.k && hist.series[params.k] ? params.k : (hist.series[caseKey("Partition", "PDQSort", 8192, "Random")] ? caseKey("Partition", "PDQSort", 8192, "Random") : keys[0])).split("|");
  const runs = index.runs;
  const models = [...new Set(runs.map(r => cpuModel(r.cpu)))];

  root.innerHTML = `
    <section class="panel">
      <h2>History</h2>
      ${intro("One case across every published run. Compare only points of the same color: each color is one CPU model.",
        "Each point is the mean of one run, with ±StdDev as a band. A vertical rule marks a change of CPU model, and lines connect only consecutive runs on the same model. The table compares each run with the previous run on the same model; \"within error\" means the 99.9% confidence intervals overlap.")}
      <div class="filters" id="h-f"></div>
      <div class="legend">${models.map((mdl, i) => `<span><span class="sw" style="background:${SER(i)}"></span>${esc(mdl)}</span>`).join("")}<span>vertical rule = CPU model changed</span></div>
      <div class="scroll" id="h-c"></div>
      <div class="vscroll" id="h-t" style="max-height:360px"></div>
    </section>`;

  const drawFilters = () => {
    const ms = [...new Set(parts.filter(q => q[0] === c).map(q => q[1]))].sort();
    if (!ms.includes(m)) m = ms[0];
    const ns = [...new Set(parts.filter(q => q[0] === c && q[1] === m).map(q => +q[2]))].sort((a, b) => a - b);
    if (!ns.includes(+n)) n = String(ns.at(-1));
    const ps = PATTERNS.filter(x => hist.series[caseKey(c, m, n, x)]);
    if (!ps.includes(p)) p = ps[0];
    root.querySelector("#h-f").innerHTML = selectEl("h-c1", "Category", cats, c) + selectEl("h-m", "Method", ms, m, "wide") + selectEl("h-n", "Size", ns.map(v => [v, "n = " + v]), n) + selectEl("h-p", "Pattern", ps, p);
  };

  const drawChart = () => {
    const key = caseKey(c, m, n, p);
    setParams({ k: key });
    const pts = hist.series[key].map(([i, mean, err, sd]) => ({ i, mean, err, sd, run: runs[i], model: cpuModel(runs[i].cpu) }));
    const host = root.querySelector("#h-c");
    host.innerHTML = "";
    const W = chartWidth(host, 280, 1000), M = { l: W < 560 ? 54 : 66, r: 16, t: 0, b: 52 };
    const x = d3.scalePoint().domain(runs.map((_, i) => i)).range([M.l + 10, W - M.r - 10]);
    // CPU-model changes are labeled left to right. A label moves down a row when it would overlap the
    // previous label in that row, or when its rule would cross a label already placed lower down.
    const ROW = 12, changes = [];
    runs.forEach((r, i) => {
      if (i === 0 || cpuModel(r.cpu) === cpuModel(runs[i - 1].cpu)) return;
      const xx = (x(i) + x(i - 1)) / 2, text = "→ " + cpuModel(r.cpu), x0 = xx + 4, x1 = x0 + text.length * 5.8;
      let row = 0;
      while (changes.some(l => (l.row === row && l.x1 + 6 > x0) || (l.row > row && l.x0 <= xx && xx <= l.x1))) row++;
      changes.push({ xx, x0, x1, row, text });
    });
    const labelRows = changes.length ? d3.max(changes, l => l.row) + 1 : 0;
    M.t = 16 + labelRows * ROW;
    const H = Math.round(Math.max(240, Math.min(340, W * 0.5))) + labelRows * ROW;
    const s = svg(host, W, H, `History of ${key}`);
    const lo = d3.min(pts, q => q.mean - q.sd), hi = d3.max(pts, q => q.mean + q.sd);
    const y = d3.scaleLinear().domain([lo - (hi - lo) * 0.1, hi + (hi - lo) * 0.1 || hi * 1.05]).nice().range([H - M.b, M.t]);
    s.append("g").attr("class", "grid").selectAll("line").data(y.ticks(5)).join("line").attr("x1", M.l).attr("x2", W - M.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "axis").attr("transform", `translate(${M.l},0)`).call(d3.axisLeft(y).ticks(5).tickFormat(fmtT).tickSize(0).tickPadding(6));
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
      s.append("line").attr("x1", l.xx).attr("x2", l.xx).attr("y1", ty - 8).attr("y2", H - M.b).style("stroke", "var(--v-ink-2)");
      s.append("text").attr("x", l.x0).attr("y", ty).attr("class", "t-muted").style("font-size", "10px").text(l.text);
    });
    let seg = [];
    const flush = () => { if (seg.length > 1) s.append("path").attr("d", d3.line().x(q => x(q.i)).y(q => y(q.mean))(seg)).style("fill", "none").style("stroke", SER(models.indexOf(seg[0].model))).style("stroke-width", 2); seg = []; };
    pts.forEach(q => {
      if (seg.length && (seg[0].model !== q.model || runs.slice(seg.at(-1).i + 1, q.i).some(r => cpuModel(r.cpu) !== q.model))) flush();
      seg.push(q);
    });
    flush();
    const tipOf = q => `<b>${esc(fmtDate(q.run.timestamp))}</b><br>${esc(q.run.cpu)}<br>Mean <b class="mono">${fmtT(q.mean)}</b> ± SD ${fmtT(q.sd)}${q.run.commit ? `<br><span class="mono">${esc(q.run.commit.slice(0, 7))}</span>` : ""}`;
    pts.forEach(q => {
      const col = SER(models.indexOf(q.model));
      s.append("line").attr("x1", x(q.i)).attr("x2", x(q.i)).attr("y1", y(q.mean - q.sd)).attr("y2", y(q.mean + q.sd)).style("stroke", col).style("stroke-width", 6).style("stroke-opacity", 0.25).style("stroke-linecap", "round");
      s.append("circle").attr("cx", x(q.i)).attr("cy", y(q.mean)).attr("r", 4.5).style("fill", col).style("stroke", "var(--panel)").style("stroke-width", 2);
    });
    const ring = s.append("circle").attr("r", 8).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5).style("display", "none");
    const hover = ev => {
      const [px] = d3.pointer(ev, s.node());
      const q = d3.least(pts, o => Math.abs(x(o.i) - px));
      ring.attr("cx", x(q.i)).attr("cy", y(q.mean)).style("display", null);
      showTip(ev, tipOf(q));
    };
    s.append("rect").attr("x", M.l).attr("y", M.t).attr("width", W - M.l - M.r).attr("height", H - M.t - M.b).style("fill", "transparent")
      .on("pointermove", hover).on("pointerdown", hover).on("pointerleave", () => { ring.style("display", "none"); hideTip(); });

    let t = `<table class="st"><thead><tr><th>Run</th><th>CPU</th><th>Mean</th><th class="sm-hide">Error</th><th>CV</th><th>Change<span class="sm-hide"> vs previous run, same CPU</span></th><th class="sm-hide">Source</th></tr></thead><tbody>`;
    const rows = [...pts].reverse();
    rows.forEach(q => {
      const prev = pts.filter(o => o.i < q.i && o.model === q.model).at(-1);
      let change = "—";
      if (prev) {
        const d = q.mean / prev.mean - 1;
        const sig = prev.err != null && q.err != null ? (q.mean - q.err > prev.mean + prev.err ? "slower" : q.mean + q.err < prev.mean - prev.err ? "faster" : "within error") : "";
        change = `${d >= 0 ? "+" : ""}${(d * 100).toFixed(1)}%${sig ? ` <span class="sig">${sig}</span>` : ""}`;
      }
      const link = q.run.runUrl ? `<a href="${esc(q.run.runUrl)}">${q.run.source === "readme" ? "README import" : "workflow run"}</a>` : esc(q.run.source);
      t += `<tr${q.run.id === run.entry.id ? ' class="on"' : ""}><td><span class="md-hide">${esc(fmtDate(q.run.timestamp))}</span><span class="md-only">${esc(fmtDate(q.run.timestamp).slice(5, 16))}</span></td><td title="${esc(q.run.cpu)}"><span class="md-hide">${esc(q.run.cpu)}</span><span class="md-only">${esc(q.model)}</span></td><td class="num">${fmtT(q.mean)}</td><td class="num sm-hide">${q.err == null ? "—" : "±" + fmtT(q.err)}</td><td class="num">${pct(q.sd / q.mean)}</td><td class="num chg">${change}</td><td class="sm-hide">${link}</td></tr>`;
    });
    root.querySelector("#h-t").innerHTML = t + "</tbody></table>";
  };

  root.querySelector("#h-f").addEventListener("input", e => {
    const id = e.target.id, v = e.target.value;
    if (id === "h-c1") c = v; else if (id === "h-m") m = v; else if (id === "h-n") n = v; else if (id === "h-p") p = v;
    drawFilters(); drawChart();
  });
  drawFilters();
  drawChart();
  onResize(drawChart);
}
