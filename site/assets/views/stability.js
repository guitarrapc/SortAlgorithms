import { d3, PATTERNS, fmtT, pct, esc, svg, logTicks, showTip, hideTip, caseTip, frontier, selectEl, leagueSizes as sizesForLeagues, defaultLeague, intro, foldFilters, chartWidth, SCALING, homeOf, placeLabels } from "../lib.js";

// Speed against run-to-run variation for one size league, then the per-iteration measurements
// behind the CV for one category.
export function render(root, { run, params, setParams, onResize }) {
  const leagueSizes = sizesForLeagues(run);
  const n0 = leagueSizes.includes(+params.n) ? +params.n : defaultLeague(run);
  const p0 = PATTERNS.includes(params.p) ? params.p : "Random";
  // A case measured by the Scaling class belongs to the class of its algorithm; the reference belongs to every class.
  const homes = run.categories.filter(c => c !== SCALING);
  const inClass = (r, c) => r.c === c || (r.c === SCALING && (r.ref || homeOf(run, r.m) === c));
  const c0 = homes.includes(params.c) ? params.c : (homes.includes("Partition") ? "Partition" : homes[0]);
  root.innerHTML = `
    <section class="panel">
      <h2>Stability</h2>
      ${intro("Speed against run-to-run variation (CV). Algorithms on the line are not beaten on both by any other.",
        "Each point is one algorithm: further left is faster, lower is more stable. The CV axis uses a square-root scale so that one noisy case does not flatten the rest.",
        "CV here comes from a single CI run of a few iterations, so check the history before treating a high CV as a property of the algorithm.")}
      <div class="filters">
        ${selectEl("s-n", "League", leagueSizes.map(s => [s, "n = " + s]), n0)}
        ${selectEl("s-p", "Pattern", PATTERNS, p0)}
        ${selectEl("s-h", "Highlight", [["", "Frontier"], ...homes.map(c => [c, c])], homes.includes(params.h) ? params.h : "", "wide")}
      </div>
      <div class="legend" id="s-l"></div>
      <div class="scroll" id="s-c"></div>
      <p class="note" id="s-fl"></p>
    </section>
    <section class="panel">
      <h3>Per-iteration measurements</h3>
      <div class="filters">${selectEl("s-c2", "Category", homes, c0)}</div>
      <div class="note" id="s-dn"></div>
      <div class="scroll" id="s-d"></div>
    </section>`;
  foldFilters(root.querySelector(".panel"));

  const drawPareto = () => {
    const n = +root.querySelector("#s-n").value, p = root.querySelector("#s-p").value, hl = root.querySelector("#s-h").value;
    setParams({ n, p, h: hl || undefined });
    const list = run.slice("*", n, p).filter(r => !r.ref);
    const refs = run.slice("*", n, p).filter(r => r.ref);
    const ref = refs.find(r => r.c === "Partition") || refs[0];
    const f = frontier(list);
    root.querySelector("#s-l").innerHTML = `<span><span class="sw" style="background:var(--s1)"></span>${hl ? esc(hl) : "Frontier"}</span><span><span class="sw" style="background:var(--v-muted);opacity:.5"></span>Other</span>${ref ? `<span><span class="sw" style="border:1.5px solid var(--v-ink)"></span>Reference (${esc(ref.m)})</span>` : ""}<span>${list.length} algorithms · hover or tap a point</span>`;
    const host = root.querySelector("#s-c");
    host.innerHTML = "";
    const W = chartWidth(host, 280, 1000), small = W < 560;
    const H = Math.round(Math.max(260, Math.min(440, W * 0.55))), M = { l: small ? 44 : 54, r: small ? 12 : 150, t: 30, b: 40 };
    const s = svg(host, W, H, "Speed and stability");
    const pts = ref ? [...list, ref] : list;
    const x = d3.scaleLog().domain(d3.extent(pts, r => r.mean)).nice().range([M.l, W - M.r]);
    // Square-root scale: one noisy case must not flatten the sub-percent CVs of everything else.
    // The axis ends just above the largest CV, so a run where every case is stable still spreads its points.
    const y = d3.scaleSqrt().domain([0, Math.max(0.004, d3.max(pts, r => r.cv) * 1.1)]).range([H - M.b, M.t]);
    const yTicks = [0, 0.0025, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.5].filter(v => v <= y.domain()[1]);
    s.append("g").attr("class", "grid").selectAll("line").data(yTicks).join("line").attr("x1", M.l).attr("x2", W - M.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${H - M.b})`).call(d3.axisBottom(x).tickValues(logTicks(x, small ? 4 : 7)).tickFormat(fmtT).tickSizeOuter(0));
    s.append("g").attr("class", "axis").attr("transform", `translate(${M.l},0)`).call(d3.axisLeft(y).tickValues(yTicks).tickFormat(d3.format(".2~%")).tickSize(0).tickPadding(6));
    s.append("text").attr("x", W - M.r).attr("y", H - 6).attr("text-anchor", "end").attr("class", "t-muted").text("Mean (log) → slower");
    s.append("text").attr("x", M.l + 4).attr("y", M.t - 14).attr("class", "t-muted").text("↑ CV, square-root scale (less stable)");
    const fl = [...f].sort((a, b) => a.mean - b.mean);
    const step = [];
    fl.forEach((r, i) => { step.push([x(r.mean), y(r.cv)]); if (fl[i + 1]) step.push([x(fl[i + 1].mean), y(r.cv)]); });
    if (step.length > 1) s.append("path").attr("d", d3.line()(step)).style("fill", "none").style("stroke", "var(--v-ink-2)").style("stroke-width", 1.5);
    const on = r => hl ? !r.ref && inClass(r, hl) : f.has(r);
    s.append("g").selectAll("circle").data([...list].sort((a, b) => on(a) - on(b))).join("circle")
      .attr("cx", r => x(r.mean)).attr("cy", r => y(r.cv)).attr("r", r => on(r) ? 5 : 3.6)
      .style("fill", r => on(r) ? "var(--s1)" : "var(--v-muted)").style("fill-opacity", r => on(r) ? 1 : 0.5)
      .style("stroke", "var(--panel)").style("stroke-width", r => on(r) ? 2 : 1);
    if (ref) s.append("circle").attr("cx", x(ref.mean)).attr("cy", y(ref.cv)).attr("r", 5.5).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5);
    const labeled = hl ? list.filter(on) : fl;
    // Small charts list the names below instead: labels would cover the points.
    root.querySelector("#s-fl").innerHTML = small && labeled.length ? `${hl ? esc(hl) : "Frontier"}, fastest first: ${[...labeled].sort((a, b) => a.mean - b.mean).map(r => esc(r.m)).join(" · ")}` : "";
    if (!small && labeled.length <= 14) {
      placeLabels(s, [...labeled].sort((a, b) => a.mean - b.mean).map(r => ({ px: x(r.mean), py: y(r.cv), text: r.m })),
        pts.map(r => [x(r.mean), y(r.cv)]), d3.pairs(step), { x0: M.l, x1: W - 2, y0: M.t - 6, y1: H - M.b });
    }
    const del = d3.Delaunay.from(pts, r => x(r.mean), r => y(r.cv));
    const ring = s.append("circle").attr("r", 8).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5).style("display", "none");
    const hover = ev => {
      const [px, py] = d3.pointer(ev, s.node());
      const r = pts[del.find(px, py)];
      ring.attr("cx", x(r.mean)).attr("cy", y(r.cv)).style("display", null);
      showTip(ev, caseTip(r) + (f.has(r) ? "<br><b>On the frontier</b>" : ""));
    };
    s.append("rect").attr("x", M.l).attr("y", M.t).attr("width", Math.min(W - M.l, W - M.l - M.r + 40)).attr("height", H - M.t - M.b).style("fill", "transparent")
      .on("pointermove", hover).on("pointerdown", hover)
      .on("pointerleave", () => { ring.style("display", "none"); hideTip(); });
  };

  const drawDistribution = () => {
    const n = +root.querySelector("#s-n").value, p = root.querySelector("#s-p").value, c = root.querySelector("#s-c2").value;
    setParams({ c });
    const host = root.querySelector("#s-d");
    host.innerHTML = "";
    const note = root.querySelector("#s-dn");
    const rows = run.slice("*", n, p).filter(r => inClass(r, c) && r.ms.length > 0).sort((a, b) => b.cv - a.cv);
    if (!run.slice("*", n, p).some(r => !r.ref && inClass(r, c))) { note.textContent = `${c} is not measured at n = ${n}.`; return; }
    if (!rows.length) { note.textContent = "This run has no per-iteration measurements (runs imported from README history keep only summary statistics)."; return; }
    note.innerHTML = `${esc(c)} · n = ${n} · ${esc(p)}. Each dot is one measured iteration, shown as its deviation from the median; the box spans the quartiles. Sorted by CV, least stable first.`;
    const dev = r => { const med = d3.median(r.ms); return r.ms.map(v => v / med - 1); };
    const ext = Math.min(0.25, d3.max(rows.flatMap(dev), v => Math.abs(v)) * 1.15 || 0.01);
    const W = chartWidth(host, 280, 1000), wide = W >= 640;
    const rowH = wide ? 30 : 44, L = wide ? Math.min(220, W * 0.24) : 8, R = wide ? Math.min(250, W * 0.27) : 8, T = 22;
    const stripY = wide ? 0 : 9;
    const H = T + rows.length * rowH + 32;
    const s = svg(host, W, H, "Per-iteration measurements");
    const x = d3.scaleLinear().domain([-ext, ext]).range([L, W - R]).clamp(true);
    s.append("g").attr("class", "grid").selectAll("line").data(x.ticks(6)).join("line").attr("x1", x).attr("x2", x).attr("y1", T - 4).attr("y2", T + rows.length * rowH);
    s.append("line").attr("x1", x(0)).attr("x2", x(0)).attr("y1", T - 8).attr("y2", T + rows.length * rowH).style("stroke", "var(--v-ink-2)");
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${T + rows.length * rowH + 2})`).call(d3.axisBottom(x).ticks(6).tickFormat(d3.format("+.0%")).tickSizeOuter(0));
    s.append("text").attr("x", wide ? W - R + 14 : W - 8).attr("y", 12).attr("text-anchor", wide ? "start" : "end").attr("class", "t-muted").text("Mean ± StdDev (CV)");
    rows.forEach((r, i) => {
      const top = T + i * rowH, cy = top + rowH / 2 + stripY, dv = dev(r).sort((a, b) => a - b);
      const q1 = d3.quantile(dv, 0.25), q3 = d3.quantile(dv, 0.75);
      const tipOf = ev => showTip(ev, `${caseTip(r)}<br><span>${r.ms.map(fmtT).join(", ")}</span>`);
      const g = s.append("g").on("pointermove", tipOf).on("pointerdown", tipOf).on("pointerleave", hideTip);
      g.append("rect").attr("x", 0).attr("y", top).attr("width", W).attr("height", rowH).style("fill", "transparent");
      g.append("text").attr("x", wide ? L - 12 : L).attr("y", wide ? cy + 4 : top + 12).attr("text-anchor", wide ? "end" : "start").attr("class", r.ref ? "t-muted" : "t-ink").text(r.m);
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
      g.append("text").attr("x", wide ? W - R + 14 : W - 8).attr("y", wide ? cy + 4 : top + 12).attr("text-anchor", wide ? "start" : "end").text(`${fmtT(r.mean)} ± ${fmtT(r.sd)} (${pct(r.cv)})`);
    });
    if (rows.some(r => dev(r).some(v => Math.abs(v) > ext))) {
      s.append("text").attr("x", W - R).attr("y", T + rows.length * rowH + 30).attr("text-anchor", "end").attr("class", "t-muted").text(`▶ beyond ±${Math.round(ext * 100)}%`);
    }
  };

  root.querySelector("#s-n").addEventListener("input", () => { drawPareto(); drawDistribution(); });
  root.querySelector("#s-p").addEventListener("input", () => { drawPareto(); drawDistribution(); });
  root.querySelector("#s-h").addEventListener("input", drawPareto);
  root.querySelector("#s-c2").addEventListener("input", drawDistribution);
  drawPareto();
  drawDistribution();
  onResize(() => { drawPareto(); drawDistribution(); });
}
