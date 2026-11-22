import { d3, PATTERNS, fmtT, pct, esc, svg, logTicks, showTip, hideTip, caseTip, frontier, selectEl, leagueSizes as sizesForLeagues } from "../lib.js";

// Speed against run-to-run variation for one size league, then the per-iteration measurements
// behind the CV for one category.
export function render(root, { run, params, setParams }) {
  const leagueSizes = sizesForLeagues(run);
  const n0 = leagueSizes.includes(+params.n) ? +params.n : leagueSizes[0];
  const p0 = PATTERNS.includes(params.p) ? params.p : "Random";
  const c0 = run.categories.includes(params.c) ? params.c : (run.categories.includes("Partition") ? "Partition" : run.categories[0]);
  root.innerHTML = `
    <section class="panel">
      <h2>Stability</h2>
      <p class="lead">Each point is one algorithm. Points on the line are not beaten on both speed and CV by any other algorithm. CV here comes from a single CI run of a few iterations, so check the history before treating a high CV as a property of the algorithm.</p>
      <div class="filters">
        ${selectEl("s-n", "League", leagueSizes.map(s => [s, "n = " + s]), n0)}
        ${selectEl("s-p", "Pattern", PATTERNS, p0)}
        ${selectEl("s-h", "Highlight", [["", "Frontier"], ...run.categories.map(c => [c, c])], params.h || "")}
      </div>
      <div class="legend" id="s-l"></div>
      <div class="scroll" id="s-c"></div>
    </section>
    <section class="panel">
      <h3>Per-iteration measurements</h3>
      <div class="filters">${selectEl("s-c2", "Category", run.categories, c0)}</div>
      <div class="note" id="s-dn"></div>
      <div class="scroll" id="s-d"></div>
    </section>`;

  const drawPareto = () => {
    const n = +root.querySelector("#s-n").value, p = root.querySelector("#s-p").value, hl = root.querySelector("#s-h").value;
    setParams({ n, p, h: hl || undefined });
    const list = run.slice("*", n, p).filter(r => !r.ref);
    const refs = run.slice("*", n, p).filter(r => r.ref);
    const ref = refs.find(r => r.c === "Partition") || refs[0];
    const f = frontier(list);
    root.querySelector("#s-l").innerHTML = `<span><span class="sw" style="background:var(--s1)"></span>${hl ? esc(hl) : "Frontier"}</span><span><span class="sw" style="background:var(--v-muted);opacity:.5"></span>Other</span>${ref ? `<span><span class="sw" style="border:1.5px solid var(--v-ink)"></span>Reference (${esc(ref.m)})</span>` : ""}<span>${list.length} algorithms · hover for details</span>`;
    const host = root.querySelector("#s-c");
    host.innerHTML = "";
    const W = 900, H = 420, M = { l: 54, r: 170, t: 18, b: 40 };
    const s = svg(host, W, H, "Speed and stability");
    const pts = ref ? [...list, ref] : list;
    const x = d3.scaleLog().domain(d3.extent(pts, r => r.mean)).nice().range([M.l, W - M.r]);
    // Square-root scale: one noisy case must not flatten the sub-percent CVs of everything else.
    const y = d3.scaleSqrt().domain([0, Math.max(0.03, d3.max(pts, r => r.cv) * 1.05)]).range([H - M.b, M.t]);
    const yTicks = [0, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.5].filter(v => v <= y.domain()[1]);
    s.append("g").attr("class", "grid").selectAll("line").data(yTicks).join("line").attr("x1", M.l).attr("x2", W - M.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${H - M.b})`).call(d3.axisBottom(x).tickValues(logTicks(x)).tickFormat(fmtT).tickSizeOuter(0));
    s.append("g").attr("class", "axis").attr("transform", `translate(${M.l},0)`).call(d3.axisLeft(y).tickValues(yTicks).tickFormat(d3.format(".1~%")).tickSize(0).tickPadding(6));
    s.append("text").attr("x", W - M.r).attr("y", H - 6).attr("text-anchor", "end").attr("class", "t-muted").text("Mean (log) → slower");
    s.append("text").attr("x", M.l + 4).attr("y", M.t + 2).attr("class", "t-muted").text("↑ CV, square-root scale (less stable)");
    const fl = [...f].sort((a, b) => a.mean - b.mean);
    const step = [];
    fl.forEach((r, i) => { step.push([x(r.mean), y(r.cv)]); if (fl[i + 1]) step.push([x(fl[i + 1].mean), y(r.cv)]); });
    if (step.length > 1) s.append("path").attr("d", d3.line()(step)).style("fill", "none").style("stroke", "var(--v-ink-2)").style("stroke-width", 1.5);
    const on = r => hl ? r.c === hl : f.has(r);
    s.append("g").selectAll("circle").data([...list].sort((a, b) => on(a) - on(b))).join("circle")
      .attr("cx", r => x(r.mean)).attr("cy", r => y(r.cv)).attr("r", r => on(r) ? 5 : 3.6)
      .style("fill", r => on(r) ? "var(--s1)" : "var(--v-muted)").style("fill-opacity", r => on(r) ? 1 : 0.5)
      .style("stroke", "var(--panel)").style("stroke-width", r => on(r) ? 2 : 1);
    if (ref) s.append("circle").attr("cx", x(ref.mean)).attr("cy", y(ref.cv)).attr("r", 5.5).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5);
    // Labels sit above-right of their dot and move up past overlapping labels; moved labels get a leader line.
    const labeled = hl ? list.filter(on) : fl;
    if (labeled.length <= 14) {
      const placed = [];
      for (const r of [...labeled].sort((a, b) => a.mean - b.mean)) {
        const px = x(r.mean), py = y(r.cv), x0 = px + 7, x1 = x0 + r.m.length * 6.2, y0 = Math.min(py - 9, H - M.b - 6);
        let ly = y0;
        while (placed.some(q => q.x0 < x1 && x0 < q.x1 && Math.abs(q.y - ly) < 13) && ly > M.t + 24) ly -= 13;
        placed.push({ x0, x1, y: ly });
        if (y0 - ly > 3) s.append("line").attr("x1", px + 2).attr("y1", py - 4).attr("x2", x0 - 1).attr("y2", ly + 2).style("stroke", "var(--v-muted)").style("stroke-width", 0.8);
        s.append("text").attr("x", x0).attr("y", ly + 4).attr("class", "t-ink").style("font-size", "10.5px").text(r.m);
      }
    }
    const del = d3.Delaunay.from(pts, r => x(r.mean), r => y(r.cv));
    const ring = s.append("circle").attr("r", 8).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5).style("display", "none");
    s.append("rect").attr("x", M.l).attr("y", M.t).attr("width", W - M.l - M.r + 40).attr("height", H - M.t - M.b).style("fill", "transparent")
      .on("pointermove", ev => {
        const [px, py] = d3.pointer(ev, s.node());
        const r = pts[del.find(px, py)];
        ring.attr("cx", x(r.mean)).attr("cy", y(r.cv)).style("display", null);
        showTip(ev, caseTip(r) + (f.has(r) ? "<br><b>On the frontier</b>" : ""));
      })
      .on("pointerleave", () => { ring.style("display", "none"); hideTip(); });
  };

  const drawDistribution = () => {
    const n = +root.querySelector("#s-n").value, p = root.querySelector("#s-p").value, c = root.querySelector("#s-c2").value;
    setParams({ c });
    const host = root.querySelector("#s-d");
    host.innerHTML = "";
    const note = root.querySelector("#s-dn");
    const rows = run.slice(c, n, p).filter(r => r.ms.length > 0).sort((a, b) => b.cv - a.cv);
    if (!run.sizesOf(c).includes(n)) { note.textContent = `${c} is not measured at n = ${n}.`; return; }
    if (!rows.length) { note.textContent = "This run has no per-iteration measurements (runs imported from README history keep only summary statistics)."; return; }
    note.innerHTML = `${esc(c)} · n = ${n} · ${esc(p)}. Each dot is one measured iteration, shown as its deviation from the median; the box spans the quartiles. Sorted by CV, least stable first.`;
    const dev = r => { const med = d3.median(r.ms); return r.ms.map(v => v / med - 1); };
    const ext = Math.min(0.25, d3.max(rows.flatMap(dev), v => Math.abs(v)) * 1.15 || 0.01);
    const W = 900, rowH = 30, L = 220, R = 250, T = 22;
    const H = T + rows.length * rowH + 32;
    const s = svg(host, W, H, "Per-iteration measurements");
    const x = d3.scaleLinear().domain([-ext, ext]).range([L, W - R]).clamp(true);
    s.append("g").attr("class", "grid").selectAll("line").data(x.ticks(6)).join("line").attr("x1", x).attr("x2", x).attr("y1", T - 4).attr("y2", T + rows.length * rowH);
    s.append("line").attr("x1", x(0)).attr("x2", x(0)).attr("y1", T - 8).attr("y2", T + rows.length * rowH).style("stroke", "var(--v-ink-2)");
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${T + rows.length * rowH + 2})`).call(d3.axisBottom(x).ticks(6).tickFormat(d3.format("+.0%")).tickSizeOuter(0));
    s.append("text").attr("x", W - R + 14).attr("y", 12).attr("class", "t-muted").text("Mean ± StdDev (CV)");
    rows.forEach((r, i) => {
      const cy = T + i * rowH + rowH / 2, dv = dev(r).sort((a, b) => a - b);
      const q1 = d3.quantile(dv, 0.25), q3 = d3.quantile(dv, 0.75);
      const g = s.append("g").on("pointermove", ev => showTip(ev, `${caseTip(r)}<br><span class="mono">${r.ms.map(fmtT).join(", ")}</span>`)).on("pointerleave", hideTip);
      g.append("rect").attr("x", 0).attr("y", cy - rowH / 2).attr("width", W).attr("height", rowH).style("fill", "transparent");
      g.append("text").attr("x", L - 12).attr("y", cy + 4).attr("text-anchor", "end").attr("class", r.ref ? "t-muted" : "t-ink").text(r.m);
      g.append("rect").attr("x", x(q1)).attr("y", cy - 8).attr("width", Math.max(1, x(q3) - x(q1))).attr("height", 16).attr("rx", 3).style("fill", "var(--v-band)").style("stroke", "var(--v-axis)");
      g.append("line").attr("x1", x(0)).attr("x2", x(0)).attr("y1", cy - 8).attr("y2", cy + 8).style("stroke", "var(--v-ink)").style("stroke-width", 2);
      dv.forEach((v, j) => {
        const clipped = Math.abs(v) > ext;
        g.append(clipped ? "path" : "circle")
          .attr(clipped ? "d" : "r", clipped ? d3.symbol(d3.symbolTriangle, 40)() : 3.2)
          .attr("transform", clipped ? `translate(${x(v)},${cy})rotate(${v > 0 ? 90 : -90})` : null)
          .attr("cx", clipped ? null : x(v)).attr("cy", clipped ? null : cy + ((j % 3) - 1) * 4)
          .style("fill", "var(--s1)").style("stroke", "var(--panel)").style("stroke-width", 1);
      });
      g.append("text").attr("x", W - R + 14).attr("y", cy + 4).attr("class", "t-mono").text(`${fmtT(r.mean)} ± ${fmtT(r.sd)} (${pct(r.cv)})`);
    });
    if (rows.some(r => dev(r).some(v => Math.abs(v) > ext))) {
      s.append("text").attr("x", W - R).attr("y", T + rows.length * rowH + 28).attr("text-anchor", "end").attr("class", "t-muted").style("font-size", "10px").text(`▶ beyond ±${Math.round(ext * 100)}%`);
    }
  };

  root.querySelector("#s-n").addEventListener("input", () => { drawPareto(); drawDistribution(); });
  root.querySelector("#s-p").addEventListener("input", () => { drawPareto(); drawDistribution(); });
  root.querySelector("#s-h").addEventListener("input", drawPareto);
  root.querySelector("#s-c2").addEventListener("input", drawDistribution);
  drawPareto();
  drawDistribution();
}
