import { d3, PATTERNS, PATTERN_SHORT, fmtX, esc, svg, logTicks, showTip, hideTip, geo, anchorBase, anchorFactor, pct, cvBadge, leagueSizes as sizesForLeagues } from "../lib.js";

// Overall ranking per size. Each league holds only algorithms measured at that size, so
// O(n log n) and linear sorts are never ranked against each other at a size too small to separate them.
export function render(root, { run, params, setParams }) {
  const leagueSizes = sizesForLeagues(run);
  let n = leagueSizes.includes(+params.n) ? +params.n : leagueSizes[0];
  const base = anchorBase(run);
  let corrected = params.raw !== "1" && !!base;
  let showAll = false;
  root.innerHTML = `
    <section class="panel">
      <h2>Leagues</h2>
      <p class="lead">Score = geometric mean, over the six input patterns, of each algorithm's time divided by the fastest algorithm in the league for that pattern. The tick marks the algorithm's worst pattern. The reference sort is excluded from the ranking.</p>
      <div class="filters">
        <div class="fl"><span>League</span><div class="seg" role="group" id="l-n">${leagueSizes.map(s => `<button type="button" data-v="${s}" aria-pressed="${s === n}">n = ${s}</button>`).join("")}</div></div>
        <label class="check"><input type="checkbox" id="l-corr"${corrected ? " checked" : ""}${base ? "" : " disabled"}> Correct for drift between benchmark classes with the reference sort</label>
      </div>
      <div id="l-note" class="note"></div>
      <div class="scroll" id="l-c"></div>
      <div id="l-more"></div>
    </section>`;

  const scores = () => {
    const cats = run.categories.filter(c => run.sizesOf(c).includes(n));
    const factors = new Map(cats.map(c => [c, corrected ? anchorFactor(run, c, base) : 1]));
    const list = cats.flatMap(c => run.methodsOf(c).filter(m => m !== run.reference).map(m => ({ c, m, rows: PATTERNS.map(p => run.get(c, m, n, p)) })))
      .filter(d => d.rows.every(Boolean))
      .map(d => ({ ...d, adj: d.rows.map(r => r.mean * (factors.get(d.c) ?? 1)) }));
    const best = PATTERNS.map((p, i) => d3.min(list, d => d.adj[i]));
    for (const d of list) {
      d.ratios = d.adj.map((v, i) => ({ p: PATTERNS[i], v: v / best[i] }));
      d.score = geo(d.ratios.map(r => r.v));
      d.worst = d.ratios.reduce((a, b) => b.v > a.v ? b : a);
      d.cvMax = d3.max(d.rows, r => r.cv);
    }
    const drift = corrected ? cats.filter(c => c !== base && factors.get(c) != null).map(c => ({ c, f: factors.get(c) })) : [];
    return { list: list.sort((a, b) => a.score - b.score), cats, drift };
  };

  const draw = () => {
    setParams({ n, raw: corrected ? undefined : "1" });
    const { list, cats, drift } = scores();
    const missing = corrected ? cats.filter(c => c !== base && !run.methodsOf(c).includes(run.reference)) : [];
    root.querySelector("#l-note").innerHTML = `${list.length} algorithms from ${cats.length} categories: ${cats.map(esc).join(", ")}.` +
      (corrected && drift.length ? ` Correction against ${esc(base)}, one factor per class from the reference over all its sizes and patterns: ${drift.map(d => `${esc(d.c)} ${d.f >= 1 ? "+" : "−"}${pct(Math.abs(d.f - 1))}`).join(", ")}.` : "") +
      (missing.length ? ` No reference in ${missing.map(esc).join(", ")}; those classes are not corrected.` : "");
    const shown = showAll ? list : list.slice(0, 20);
    const host = root.querySelector("#l-c");
    host.innerHTML = "";
    const W = 900, rowH = 24, L = 270, R = 230, T = 26;
    const H = T + shown.length * rowH + 30;
    const s = svg(host, W, H, `League n=${n}`);
    const x = d3.scaleLog().domain([1, Math.max(2, d3.max(shown, d => d.worst.v) * 1.1)]).range([L, W - R]);
    s.append("g").attr("class", "grid").selectAll("line").data(logTicks(x)).join("line").attr("x1", x).attr("x2", x).attr("y1", T - 4).attr("y2", T + shown.length * rowH);
    s.append("g").attr("class", "axis").attr("transform", `translate(0,${T + shown.length * rowH + 2})`).call(d3.axisBottom(x).tickValues(logTicks(x)).tickFormat(v => "×" + v).tickSizeOuter(0));
    s.append("text").attr("x", L).attr("y", 12).attr("class", "t-muted").text("bar = score (geometric mean of pattern ratios) · tick = worst pattern");
    shown.forEach((d, i) => {
      const cy = T + i * rowH + rowH / 2;
      const g = s.append("g")
        .on("pointermove", ev => showTip(ev, `<b>${esc(d.m)}</b> <span class="cat">${esc(d.c)}</span><br>${d.ratios.map(r => `<span class="mono">${PATTERN_SHORT[r.p].padEnd(10, " ")} ${fmtX(r.v)}</span>`).join("<br>")}<br>max CV ${cvBadge(d.cvMax)}`))
        .on("pointerleave", hideTip);
      g.append("rect").attr("x", 0).attr("y", cy - rowH / 2).attr("width", W).attr("height", rowH).style("fill", "transparent");
      g.append("text").attr("x", 30).attr("y", cy + 4).attr("text-anchor", "end").attr("class", "t-mono t-muted").text(i + 1);
      g.append("text").attr("x", 40).attr("y", cy + 4).attr("class", "t-ink").text(d.m);
      g.append("text").attr("x", L - 10).attr("y", cy + 4).attr("text-anchor", "end").attr("class", "t-muted").style("font-size", "10.5px").text(d.c);
      g.append("rect").attr("x", L).attr("y", cy - 6).attr("width", Math.max(2, x(d.score) - L)).attr("height", 12).attr("rx", 3).style("fill", "var(--s1)");
      g.append("line").attr("x1", x(d.worst.v)).attr("x2", x(d.worst.v)).attr("y1", cy - 8).attr("y2", cy + 8).style("stroke", "var(--v-ink)").style("stroke-width", 2);
      g.append("text").attr("x", W - R + 12).attr("y", cy + 4).attr("class", "t-mono").text(`${fmtX(d.score)}  worst ${fmtX(d.worst.v)} ${PATTERN_SHORT[d.worst.p]}`);
    });
    const more = root.querySelector("#l-more");
    more.innerHTML = list.length > 20 ? `<button class="btn" type="button">${showAll ? "Show top 20" : `Show all ${list.length}`}</button>` : "";
    const btn = more.querySelector("button");
    if (btn) btn.onclick = () => { showAll = !showAll; draw(); };
  };

  root.querySelector("#l-n").addEventListener("click", e => {
    const b = e.target.closest("button");
    if (!b) return;
    root.querySelectorAll("#l-n button").forEach(x => x.setAttribute("aria-pressed", String(x === b)));
    n = +b.dataset.v; draw();
  });
  root.querySelector("#l-corr").addEventListener("change", e => { corrected = e.target.checked; draw(); });
  draw();
}
