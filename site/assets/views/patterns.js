import { d3, PATTERNS, PATTERN_SHORT, PATTERN_HEAD, fmtT, fmtX, esc, svg, logTicks, showTip, hideTip, selectEl, geo, leagueSizes as sizesForLeagues, defaultLeague, intro, foldFilters, chartWidth, SCALING, homeOf, KEYED, anchorBase, placeLabels } from "../lib.js";

// How the input pattern changes each algorithm's time. x is the time on Random against the reference. y is the time on the other
// patterns against the algorithm's own time on Random, so it does not repeat x. x × y is the time on those patterns against the
// reference's time on Random. The matrix shows y for each pattern.
const OTHERS = PATTERNS.filter(p => p !== "Random");
const Y_MODES = [["mean", "Geomean of other patterns"], ["best", "Best pattern"], ["worst", "Worst pattern"], ...OTHERS.map(p => [p, PATTERN_SHORT[p]])];
const Y_LABEL = { mean: "Geomean of other patterns", best: "Best pattern", worst: "Worst pattern", ...PATTERN_SHORT };
// At most five characters keep five columns on a phone: two significant digits, no leading zero; the title has the full ratio.
const fmtR = v => v < 0.001 ? "<.001" : v < 1 ? v.toPrecision(2).replace(/^0/, "") : v < 10 ? v.toFixed(1) : String(Math.round(v));
// Diverging cell color: blue below ×1 (faster than on Random), red above, full strength at 32 times either way.
const shade = v => {
  const t = Math.min(1, Math.abs(Math.log10(v)) / 1.5), pc = Math.round(t * 100);
  return { cls: (v < 1 ? "dvl" : "dvh") + (pc >= 85 ? " s2" : ""), style: `background:color-mix(in srgb, var(--dv-${v < 1 ? "lo" : "hi"}) ${pc}%, var(--dv-mid))` };
};

export function render(root, { run, params, setParams, onResize }) {
  const sizes = sizesForLeagues(run);
  const ref = run.reference, base = anchorBase(run);
  const homes = run.categories.filter(c => c !== SCALING);
  let n = sizes.includes(+params.n) ? +params.n : defaultLeague(run);
  let yMode = Y_MODES.some(([v]) => v === params.y) ? params.y : "mean";
  let cls = homes.includes(params.c) ? params.c : "";
  let comparisonOnly = params.cmp === "1";
  let sel = params.m || null;
  let sortKey = "mean", dir = 1;
  const SORTS = [["mean", "Geomean of other patterns"], ["x", `Random vs ${ref}`], ...OTHERS.map(p => [p, PATTERN_SHORT[p]]), ["m", "Name"]];

  root.innerHTML = `
    <section class="panel">
      <h2>Patterns</h2>
      ${intro(`Speed on Random against the effect of the other patterns. Left of the vertical line: faster than ${esc(ref)} on Random. Below the horizontal line: faster on the other patterns than on Random.`,
        `x: time on Random ÷ ${esc(ref)}'s time on Random, in the same class. y: time on the other patterns ÷ the algorithm's own time on Random. y is their geometric mean, the best, the worst, or one pattern. y does not use ${esc(ref)}, so the axes measure different things.`,
        `An adaptive sort does less work on input that is partly in order, so it falls below ×1. A sort with an adversarial pattern rises above ×1. x × y is the time on those patterns ÷ ${esc(ref)}'s time on Random. The line joins the algorithms that no other algorithm beats both on Random and on x × y. y alone would favor a sort that is slow on Random, because y divides by that time.`,
        "Sorted, Reversed and PipeOrgan repeat one input in every iteration. At small sizes the branch predictor can learn it, so these patterns can look faster than new input of the same shape.")}
      <div class="filters">
        ${selectEl("q-n", "Size", sizes.map(s => [s, "n = " + s]), n)}
        ${selectEl("q-y", "y axis", Y_MODES, yMode, "wide")}
        ${selectEl("q-cls", "Class", [["", "All (label the frontier)"], ...homes.map(c => [c, c])], cls, "wide")}
        <label class="check"><input type="checkbox" id="q-cmp"${comparisonOnly ? " checked" : ""}> Comparison sorts only</label>
      </div>
      <div class="legend" id="q-l"></div>
      <div id="q-chart"></div>
      <p class="note" id="q-fl"></p>
    </section>
    <section class="panel">
      <h3>Pattern adaptivity</h3>
      <p class="lead">Each cell: time on that pattern ÷ the algorithm's own time on Random. Select a row to mark it in the chart.</p>
      <div class="legend"><span>faster than on Random</span><span class="dvbar" aria-hidden="true"></span><span>slower</span><span class="note">full color at 32 times faster or slower</span></div>
      <div class="filters sm-only">${selectEl("a-s", "Sort by", SORTS, sortKey, "wide")}</div>
      <div class="tbox" id="a-t"></div>
    </section>`;
  foldFilters(root.querySelector(".panel"));

  const yOf = d => d[yMode] ?? d.ratios[yMode];

  // One row per algorithm measured at n on Random and on at least one other pattern, and one reference row.
  const data = () => {
    const rows = [];
    for (const c of run.categories) {
      if (!run.sizesOf(c).includes(n)) continue;
      const refRnd = run.get(c, ref, n, "Random") ?? (base ? run.get(base, ref, n, "Random") : undefined);
      for (const m of run.methodsOf(c)) {
        const rnd = run.get(c, m, n, "Random");
        if (!rnd) continue;
        const ratios = {};
        for (const p of OTHERS) { const r = run.get(c, m, n, p); if (r) ratios[p] = r.mean / rnd.mean; }
        const ps = Object.keys(ratios);
        if (!ps.length) continue;
        const bestP = d3.least(ps, p => ratios[p]), worstP = d3.greatest(ps, p => ratios[p]);
        rows.push({
          c, m, id: c + "|" + m, home: c === SCALING ? homeOf(run, m) : c, ref: m === ref, rnd: rnd.mean,
          x: refRnd ? rnd.mean / refRnd.mean : null, ratios, mean: geo(ps.map(p => ratios[p])), best: ratios[bestP], bestP, worst: ratios[worstP], worstP,
        });
      }
    }
    const refs = rows.filter(d => d.ref);
    return { list: rows.filter(d => !d.ref && (!comparisonOnly || d.home !== KEYED)), refRow: refs.find(d => d.c === base) ?? refs[0] ?? null };
  };

  const tipOf = d => `<b>${esc(d.m)}</b> <span class="cat">${d.ref ? "reference" : esc(d.home)}</span><br>Random <b>${fmtT(d.rnd)}</b>` +
    (d.x != null && !d.ref ? ` · ${fmtX(d.x)} vs ${esc(ref)}` : "") +
    OTHERS.filter(p => d.ratios[p] != null).map(p => `<div class="tr${p === (PATTERN_SHORT[yMode] ? yMode : null) ? " on" : ""}">${PATTERN_SHORT[p]} <b>${fmtX(d.ratios[p])}</b>` +
      `${p === d.bestP ? " best" : p === d.worstP ? " worst" : ""}</div>`).join("") +
    `<div>${esc(Y_LABEL[yMode])} vs ${esc(ref)} on Random <b>${fmtX(d.x * yOf(d))}</b></div>` +
    `<span class="note">Geomean of other patterns: ${fmtX(d.mean)}${d.ref ? "" : " · click to select"}</span>`;

  const select = id => {
    sel = sel === id ? null : id;
    setParams({ m: sel ?? undefined });
    drawQuadrant();
    root.querySelectorAll("#a-t tr[data-id]").forEach(tr => tr.classList.toggle("on", tr.dataset.id === sel));
  };

  const drawQuadrant = () => {
    setParams({ n, y: yMode === "mean" ? undefined : yMode, c: cls || undefined, cmp: comparisonOnly ? "1" : undefined, m: sel ?? undefined });
    const { list, refRow } = data();
    const host = root.querySelector("#q-chart");
    host.innerHTML = "";
    const pts = list.filter(d => d.x != null && yOf(d) > 0);
    const note = root.querySelector("#q-fl");
    if (!pts.length) {
      host.innerHTML = `<p class="empty">No algorithm has Random and ${PATTERN_SHORT[yMode] ?? "another pattern"} at n = ${n}.</p>`;
      root.querySelector("#q-l").innerHTML = note.innerHTML = "";
      return;
    }
    const rp = refRow && refRow.x != null && yOf(refRow) > 0 ? refRow : null;
    const all = rp ? [...pts, rp] : pts;
    // The frontier: algorithms that no other algorithm beats both on Random (x) and on the time on the y patterns (x × y).
    const t = d => d.x * yOf(d);
    const front = new Set();
    let low = Infinity;
    for (const d of [...pts].sort((a, b) => a.x - b.x || t(a) - t(b))) if (t(d) < low) { front.add(d); low = t(d); }
    const on = d => cls ? d.home === cls : front.has(d);
    const picked = pts.find(d => d.id === sel);
    root.querySelector("#q-l").innerHTML = `<span><span class="sw" style="background:var(--s1)"></span>${cls ? esc(cls) : "Frontier"}</span><span><span class="sw" style="background:var(--v-muted);opacity:.5"></span>Other</span>` +
      (rp ? `<span><span class="sw" style="border:1.5px solid var(--v-ink)"></span>Reference (${esc(ref)})</span>` : "") + `<span>${pts.length} algorithms · hover or tap a point</span>`;

    const W = chartWidth(host, 280, 1000), small = W < 560;
    const H = Math.round(Math.max(280, Math.min(480, W * 0.62))), M = { l: small ? 44 : 54, r: small ? 12 : 150, t: 30, b: 40 };
    const s = svg(host, W, H, `Time on Random against ${Y_LABEL[yMode]} relative to Random`);
    // Both axes include ×1, so the quadrant lines are always drawn.
    const ext = vals => { const [a, b] = d3.extent([1, ...vals]); return [a / 1.25, b * 1.25]; };
    const x = d3.scaleLog().domain(ext(all.map(d => d.x))).range([M.l, W - M.r]);
    const y = d3.scaleLog().domain(ext(all.map(yOf))).range([H - M.b, M.t]);
    const xt = logTicks(x, small ? 4 : 7), yt = logTicks(y, 7), fx = v => "×" + d3.format("~g")(v);
    s.append("g").attr("class", "grid").selectAll("line").data(yt).join("line").attr("x1", M.l).attr("x2", W - M.r).attr("y1", y).attr("y2", y);
    s.append("g").attr("class", "grid").selectAll("line").data(xt).join("line").attr("x1", x).attr("x2", x).attr("y1", M.t).attr("y2", H - M.b);
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${H - M.b})`).call(d3.axisBottom(x).tickValues(xt).tickFormat(fx).tickSizeOuter(0));
    s.append("g").attr("class", "axis").attr("transform", `translate(${M.l},0)`).call(d3.axisLeft(y).tickValues(yt).tickFormat(fx).tickSize(0).tickPadding(6));
    s.append("g").attr("class", "quad").selectAll("line").data([[x(1), M.t, x(1), H - M.b], [M.l, y(1), W - M.r, y(1)]]).join("line")
      .attr("x1", q => q[0]).attr("y1", q => q[1]).attr("x2", q => q[2]).attr("y2", q => q[3]);
    // Each corner names its quadrant: speed on Random against the reference, then the effect of the other patterns.
    for (const [cx, cy, anchor, text] of [[M.l + 6, M.t + 12, "start", "faster · patterns slower"], [W - M.r - 6, M.t + 12, "end", "slower · patterns slower"],
      [M.l + 6, H - M.b - 8, "start", "faster · patterns faster"], [W - M.r - 6, H - M.b - 8, "end", "slower · patterns faster"]]) {
      s.append("text").attr("x", cx).attr("y", cy).attr("text-anchor", anchor).attr("class", "t-muted quad-l").text(text);
    }
    s.append("text").attr("x", W - M.r).attr("y", H - 6).attr("text-anchor", "end").attr("class", "t-muted").text(`Random ÷ ${ref} (log) → slower`);
    s.append("text").attr("x", M.l + 4).attr("y", M.t - 14).attr("class", "t-muted").text(`↑ ${Y_LABEL[yMode]} ÷ Random (log)`);

    const fl = [...front].sort((a, b) => a.x - b.x);
    const step = [];
    // From each frontier point the line keeps its x × y up to the next point's x, then drops to that point.
    fl.forEach((d, i) => { step.push([x(d.x), y(yOf(d))]); const e = fl[i + 1]; if (e) step.push([x(e.x), y(t(d) / e.x)]); });
    if (!cls && step.length > 1) s.append("path").attr("d", d3.line()(step)).style("fill", "none").style("stroke", "var(--v-ink-2)").style("stroke-width", 1.5);
    s.append("g").selectAll("circle").data([...pts].sort((a, b) => on(a) - on(b))).join("circle")
      .attr("cx", d => x(d.x)).attr("cy", d => y(yOf(d))).attr("r", d => on(d) ? 5 : 3.6)
      .style("fill", d => on(d) ? "var(--s1)" : "var(--v-muted)").style("fill-opacity", d => on(d) ? 1 : 0.5)
      .style("stroke", "var(--panel)").style("stroke-width", d => on(d) ? 2 : 1);
    if (rp) s.append("circle").attr("cx", x(rp.x)).attr("cy", y(yOf(rp))).attr("r", 5.5).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5);
    if (picked) s.append("circle").attr("cx", x(picked.x)).attr("cy", y(yOf(picked))).attr("r", 9).style("fill", "none").style("stroke", "var(--accent)").style("stroke-width", 2);

    const labeled = cls ? pts.filter(on) : fl;
    const named = picked && !labeled.includes(picked) ? [...labeled, picked] : labeled;
    // Small charts list the names below instead: labels would cover the points.
    note.innerHTML = small && labeled.length
      ? `${cls ? esc(cls) : "Frontier"}, fastest on Random first: ${[...labeled].sort((a, b) => a.x - b.x).map(d => esc(d.m)).join(" · ")}` + (picked ? `. Selected: <b>${esc(picked.m)}</b>` : "")
      : !small && labeled.length > 14 ? `${labeled.length} points in ${esc(cls)}, too many to label: hover or tap a point.` : "";
    if (!small && named.length <= 15) {
      placeLabels(s, [...named].sort((a, b) => a.x - b.x).map(d => ({ px: x(d.x), py: y(yOf(d)), text: d.m })),
        all.map(d => [x(d.x), y(yOf(d))]), d3.pairs(step), { x0: M.l, x1: W - 2, y0: M.t - 6, y1: H - M.b });
    }

    const del = d3.Delaunay.from(all, d => x(d.x), d => y(yOf(d)));
    const ring = s.append("circle").attr("r", 8).style("fill", "none").style("stroke", "var(--v-ink)").style("stroke-width", 1.5).style("display", "none");
    const at = ev => { const [px, py] = d3.pointer(ev, s.node()); return all[del.find(px, py)]; };
    const hover = ev => {
      const d = at(ev);
      ring.attr("cx", x(d.x)).attr("cy", y(yOf(d))).style("display", null);
      showTip(ev, tipOf(d));
    };
    s.append("rect").attr("x", M.l).attr("y", M.t).attr("width", Math.min(W - M.l, W - M.l - M.r + 40)).attr("height", H - M.t - M.b).style("fill", "transparent")
      .on("pointermove", hover).on("pointerdown", hover)
      .on("click", ev => { const d = at(ev); if (!d.ref) select(d.id); })
      .on("pointerleave", () => { ring.style("display", "none"); hideTip(); });
  };

  const drawMatrix = () => {
    const { list, refRow } = data();
    const rows = list.filter(d => !cls || d.home === cls);
    const val = (d, k) => k === "x" ? d.x : k === "mean" ? d.mean : d.ratios[k];
    rows.sort((a, b) => {
      if (sortKey === "m") return dir * a.m.localeCompare(b.m);
      const va = val(a, sortKey), vb = val(b, sortKey);
      return va == null ? (vb == null ? 0 : 1) : vb == null ? -1 : dir * (va - vb);
    });
    const arrow = k => sortKey === k ? (dir > 0 ? " ↑" : " ↓") : "";
    const th = (k, label, cl = "") => `<th data-sort="${k}"${cl ? ` class="${cl}"` : ""}>${label}${arrow(k)}</th>`;
    const cell = (v, cl = "") => {
      if (v == null) return `<td class="c${cl}">—</td>`;
      const { cls: c, style } = shade(v);
      return `<td class="c ${c}${cl}" style="${style}" title="${fmtX(v)}">${fmtR(v)}</td>`;
    };
    const tr = d => `<tr class="${d.ref ? "ref" : ""}${d.id === sel ? " on" : ""}"${d.ref ? "" : ` data-id="${esc(d.id)}"`}>` +
      `<td class="l" title="${esc(d.m)}">${esc(d.m)}${d.ref ? "" : ` <span class="cat md-hide">${esc(d.home)}</span>`}</td>` +
      `<td class="x sm-hide">${d.x == null ? "—" : fmtX(d.x)}</td>${OTHERS.map(p => cell(d.ratios[p])).join("")}${cell(d.mean, " sm-hide")}</tr>`;
    const host = root.querySelector("#a-t");
    if (!rows.length && !refRow) { host.innerHTML = `<p class="empty">Nothing is measured at n = ${n}.</p>`; return; }
    host.innerHTML = `<table class="hm pa"><thead><tr>${th("m", "Algorithm", "l")}${th("x", `Random vs ${esc(ref)}`, "sm-hide")}${OTHERS.map(p => th(p, PATTERN_HEAD[p])).join("")}${th("mean", "Geo&shy;mean", "sm-hide")}</tr></thead>` +
      `<tbody>${refRow ? tr(refRow) : ""}${rows.map(tr).join("")}</tbody></table>`;
  };

  const host = root.querySelector("#a-t");
  host.addEventListener("click", e => {
    const h = e.target.closest("th[data-sort]");
    if (h) {
      const k = h.dataset.sort;
      if (k === sortKey) dir = -dir; else { sortKey = k; dir = 1; }
      root.querySelector("#a-s").value = sortKey;
      drawMatrix();
      return;
    }
    const row = e.target.closest("tr[data-id]");
    if (row) select(row.dataset.id);
  });
  root.querySelector("#a-s").addEventListener("change", e => { sortKey = e.target.value; dir = 1; drawMatrix(); });
  const redraw = () => { drawQuadrant(); drawMatrix(); };
  root.querySelector("#q-n").addEventListener("change", e => { n = +e.target.value; redraw(); });
  root.querySelector("#q-y").addEventListener("change", e => { yMode = e.target.value; drawQuadrant(); });
  root.querySelector("#q-cls").addEventListener("change", e => { cls = e.target.value; redraw(); });
  root.querySelector("#q-cmp").addEventListener("change", e => { comparisonOnly = e.target.checked; redraw(); });
  redraw();
  onResize(drawQuadrant);
}
