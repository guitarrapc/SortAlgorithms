// Shared data access, formatting and chart helpers for the benchmark site.
// d3 is loaded as a global UMD script by index.html.
const d3 = globalThis.d3;
export { d3 };

export const PATTERNS = ["Random", "SingleElementMoved", "Sorted", "Reversed", "PipeOrgan", "ManyDuplicates"];
export const PATTERN_SHORT = { Random: "Random", SingleElementMoved: "1-Moved", Sorted: "Sorted", Reversed: "Reversed", PipeOrgan: "PipeOrgan", ManyDuplicates: "ManyDup" };
// Column headers that may wrap in narrow matrix cells (soft hyphens).
export const PATTERN_HEAD = { Random: "Random", SingleElementMoved: "1-Moved", Sorted: "Sorted", Reversed: "Re&shy;versed", PipeOrgan: "Pipe&shy;Organ", ManyDuplicates: "Many&shy;Dup" };
const CATEGORY_ORDER = ["Exchange", "Selection", "SelectionSimd", "Insertion", "Merge", "MergeSlow", "Heap", "Partition", "Distribution", "Network", "Tree", "Scaling"];
// The Scaling class measures algorithms of the other classes again at sizes no other class measures.
export const SCALING = "Scaling";
// The class an algorithm belongs to, for a case measured by the Scaling class.
export const homeOf = (run, m) => run.categories.find(c => c !== SCALING && run.methodsOf(c).includes(m)) ?? SCALING;
export const shortN = n => n >= 1 << 20 ? n / (1 << 20) + "M" : n >= 1024 ? n / 1024 + "k" : String(n);
const REPO = "https://github.com/guitarrapc/SortAlgorithms";

// ---------------------------------------------------------------- data

const cache = new Map();
async function getJson(path) {
  if (!cache.has(path)) {
    cache.set(path, fetch(path).then(r => {
      if (!r.ok) throw new Error(`${path}: HTTP ${r.status}`);
      return r.json();
    }));
  }
  return cache.get(path);
}
export const loadIndex = () => getJson("data/index.json");
export const loadHistory = () => getJson("data/history.json");
export async function loadRun(entry) { return prepare(await getJson("data/" + entry.file), entry); }

export const caseKey = (c, m, n, p) => `${c}|${m}|${n}|${p}`;

export function prepare(raw, entry) {
  const reference = raw.reference || "DotnetSort";
  const cases = raw.cases.map(x => ({
    c: x.category, m: x.method, n: x.size, p: x.pattern,
    mean: x.mean, median: x.median, sd: x.stdDev, err: x.error ?? 0, count: x.n, alloc: x.allocated, ms: x.measurements || [],
    cv: x.mean > 0 ? x.stdDev / x.mean : 0, ref: x.method === reference,
  }));
  const idx = new Map(cases.map(x => [caseKey(x.c, x.m, x.n, x.p), x]));
  const present = new Set(cases.map(x => x.c));
  const categories = [...CATEGORY_ORDER.filter(c => present.has(c)), ...[...present].filter(c => !CATEGORY_ORDER.includes(c)).sort()];
  const sizes = new Map(categories.map(c => [c, [...new Set(cases.filter(x => x.c === c).map(x => x.n))].sort((a, b) => a - b)]));
  const methods = new Map(categories.map(c => [c, [...new Set(cases.filter(x => x.c === c).map(x => x.m))]]));
  const allSizes = [...new Set(cases.map(x => x.n))].sort((a, b) => a - b);
  return {
    raw, entry, reference, cases, categories, allSizes,
    get: (c, m, n, p) => idx.get(caseKey(c, m, n, p)),
    sizesOf: c => sizes.get(c) || [],
    methodsOf: c => methods.get(c) || [],
    maxSize: c => (sizes.get(c) || []).at(-1),
    slice: (c, n, p) => cases.filter(x => (c === "*" || x.c === c) && x.n === n && x.p === p),
    algo: m => (raw.algorithms && raw.algorithms[m]) || null,
    hasMeasurements: cases.some(x => x.ms.length > 0),
  };
}

// "AMD EPYC 9V74 3.69GHz" -> "EPYC 9V74". The reported clock varies between runners of the same model.
export const cpuModel = s => (s || "unknown").replace(/^(AMD|Intel\(R\)|Intel)\s+/, "").replace(/\s+[\d.]+\s*GHz.*$/, "").trim();
// The JIT's x86-64 level by the widest vector instructions it allows.
const ISA = { "x86-64-v4": "AVX-512", "x86-64-v3": "AVX2", "x86-64-v2": "SSE4.2" };
export const isaName = isa => isa ? ISA[isa] ?? isa : "";
// Runs are comparable only on the same CPU model and instruction set: runners of one model differ in AVX-512.
export const machineOf = entry => cpuModel(entry.cpu) + (entry.isa ? " · " + isaName(entry.isa) : "");
// The Distribution class holds the sorts that read the key's value (radix, counting, bucket) instead of only
// comparing; they need integer or numeric keys.
export const KEYED = "Distribution";

// Sizes measured by at least two categories, and the sizes only the Scaling class measures, largest first;
// every size when no size qualifies.
export function leagueSizes(run) {
  const shared = run.allSizes.filter(n => run.categories.filter(c => run.sizesOf(c).includes(n)).length >= 2);
  const large = run.sizesOf(SCALING).filter(n => !shared.includes(n));
  return (shared.length ? [...shared, ...large].sort((a, b) => a - b) : run.allSizes).slice().reverse();
}
// The league shown first: the largest size that two or more categories measure.
export const defaultLeague = run => leagueSizes(run).find(n => run.categories.filter(c => run.sizesOf(c).includes(n)).length >= 2) ?? leagueSizes(run)[0];

// Anchor-based correction: one factor per category, the geometric mean of base/category reference
// times over every size and pattern both measured. A per-cell factor would carry the noise of a single
// reference measurement into every algorithm of the category.
export function anchorFactor(run, c, base) {
  if (c === base) return 1;
  const ratios = run.sizesOf(c).flatMap(n => PATTERNS.map(p => {
    const a = run.get(base, run.reference, n, p), b = run.get(c, run.reference, n, p);
    return a && b ? a.mean / b.mean : null;
  })).filter(v => v != null);
  return ratios.length ? geo(ratios) : null;
}
export function anchorBase(run) {
  const withRef = run.categories.filter(c => run.methodsOf(c).includes(run.reference));
  return withRef.includes("Partition") ? "Partition" : withRef.sort((a, b) => run.sizesOf(b).length - run.sizesOf(a).length)[0] || null;
}

// ---------------------------------------------------------------- math

export const geo = a => Math.exp(d3.mean(a, v => Math.log(v)));
export function fitExponent(points) {
  const xs = points.map(q => Math.log(q[0])), ys = points.map(q => Math.log(q[1]));
  const mx = d3.mean(xs), my = d3.mean(ys);
  return d3.sum(xs, (v, i) => (v - mx) * (ys[i] - my)) / d3.sum(xs, v => (v - mx) ** 2);
}
export function frontier(list) {
  const sorted = [...list].sort((a, b) => a.mean - b.mean);
  const out = new Set();
  let best = Infinity;
  for (const r of sorted) if (r.cv < best) { out.add(r); best = r.cv; }
  return out;
}

// ---------------------------------------------------------------- formatting

const f3 = v => d3.format(".3~r")(v);
export const fmtT = ns => ns == null || Number.isNaN(ns) ? "—" : ns < 1e3 ? f3(ns) + " ns" : ns < 1e6 ? f3(ns / 1e3) + " μs" : ns < 1e9 ? f3(ns / 1e6) + " ms" : f3(ns / 1e9) + " s";
// Three significant digits, so ratios below ×1 (faster than the reference) keep their order visible.
export const fmtX = r => "×" + (r < 1 ? r.toPrecision(3) : r < 10 ? r.toFixed(2) : r < 100 ? r.toFixed(1) : Math.round(r));
export const pct = v => (v * 100).toFixed(1) + "%";
export const fmtDate = iso => iso ? iso.replace("T", " ").replace(/:\d\dZ$/, " UTC") : "";
export const esc = s => String(s ?? "").replace(/[&<>"]/g, ch => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;" })[ch]);
export const status = cv => cv < 0.02 ? "good" : cv < 0.05 ? "warn" : "crit";
const ICON = { good: "●", warn: "▲", crit: "■" };
const LABEL = { good: "stable", warn: "noisy", crit: "unstable" };
export const cvBadge = cv => { const s = status(cv); return `<span class="cv ${s}" title="CV: ${LABEL[s]}"><i aria-hidden="true">${ICON[s]}</i>${pct(cv)}</span>`; };
export const allocBadge = a => a == null ? `<span class="alloc">—</span>` : a === 0 ? `<span class="alloc"><i aria-hidden="true">✓</i>0 B</span>` : `<span class="alloc">${d3.format(".3~s")(a)}B</span>`;
export const refChip = r => r.ref ? `<span class="refchip" title="Same .NET sort in every benchmark class; used to compare classes">reference</span>` : "";
export const SER = i => `var(--s${(i % 8) + 1})`;
export const sourceUrl = file => file ? `${REPO}/blob/main/${file}` : null;

// ---------------------------------------------------------------- tooltip

const tip = () => document.getElementById("tip");
export function showTip(ev, html) {
  const t = tip();
  t.innerHTML = html; t.hidden = false;
  const r = t.getBoundingClientRect();
  let x = ev.clientX + 14, y = ev.clientY + 14;
  if (x + r.width > innerWidth - 8) x = ev.clientX - r.width - 14;
  if (y + r.height > innerHeight - 8) y = ev.clientY - r.height - 14;
  t.style.left = Math.max(8, x) + "px"; t.style.top = Math.max(8, y) + "px";
}
export function hideTip() { tip().hidden = true; }
export const caseTip = r => `<b>${esc(r.m)}</b> <span class="cat">${esc(r.c)}</span><br><span class="mono">n=${r.n} · ${esc(r.p)}</span><br>Mean <b class="mono">${fmtT(r.mean)}</b> ± ${fmtT(r.err)}<br>CV ${cvBadge(r.cv)} · Alloc ${allocBadge(r.alloc)}`;

// ---------------------------------------------------------------- layout

// Phone layout, matching the 600px breakpoint in style.css.
export const narrow = () => matchMedia("(max-width: 600px)").matches;
// One visible sentence, and the full explanation in a disclosure that starts open only on wide screens.
export function intro(short, ...more) {
  return `<p class="lead">${short}</p>` + (more.length
    ? `<details class="more"${matchMedia("(min-width: 761px)").matches ? " open" : ""}><summary>How to read this</summary>${more.map(p => `<p>${p}</p>`).join("")}</details>` : "");
}
// Charts are drawn at the pixel width of their container, so text stays at its CSS size on every screen.
export const chartWidth = (host, min = 280, max = 1000) => Math.round(Math.max(min, Math.min(max, host.clientWidth || max)));

// ---------------------------------------------------------------- controls

export function selectEl(id, label, options, value, cls = "") {
  return `<label for="${id}"${cls ? ` class="${cls}"` : ""}>${esc(label)}<select id="${id}">${options.map(o => {
    const [v, t] = Array.isArray(o) ? o : [o, o];
    return `<option value="${esc(v)}"${String(v) === String(value) ? " selected" : ""}>${esc(t)}</option>`;
  }).join("")}</select></label>`;
}
export function seg(id, label, options, value) {
  return `<div class="fl"><span>${esc(label)}</span><div class="seg" role="group" id="${id}">${options.map(([v, t]) =>
    `<button type="button" data-v="${esc(v)}" aria-pressed="${String(v) === String(value)}">${esc(t)}</button>`).join("")}</div></div>`;
}
export function bindSeg(root, id, fn) {
  const g = root.querySelector("#" + id);
  g.addEventListener("click", e => {
    const b = e.target.closest("button");
    if (!b) return;
    g.querySelectorAll("button").forEach(x => x.setAttribute("aria-pressed", String(x === b)));
    fn(b.dataset.v);
  });
}

// ---------------------------------------------------------------- charts

export function svg(el, w, h, label) {
  return d3.select(el).append("svg").attr("class", "chart").attr("viewBox", `0 0 ${w} ${h}`).attr("role", "img").attr("aria-label", label || "");
}
export function logTicks(scale, max = 7) {
  const [a, b] = scale.domain();
  const lo = Math.min(a, b), hi = Math.max(a, b), out = [];
  for (let e = Math.floor(Math.log10(lo)); e <= Math.ceil(Math.log10(hi)); e++)
    for (const m of [1, 2, 5]) { const v = m * 10 ** e; if (v >= lo && v <= hi) out.push(v); }
  if (out.length > max) return out.filter(v => /^1/.test(v.toExponential()));
  return out.length >= 2 ? out : scale.ticks(4);
}
// Moves labels at least gap apart, and back above hi when the last one would fall below it.
export function nudge(labels, gap = 12, lo = -Infinity, hi = Infinity) {
  labels.sort((a, b) => a.y - b.y);
  for (let i = 0; i < labels.length; i++) labels[i].y = Math.max(labels[i].y, i ? labels[i - 1].y + gap : lo);
  for (let i = labels.length - 1; i >= 0; i--) labels[i].y = Math.min(labels[i].y, i < labels.length - 1 ? labels[i + 1].y - gap : hi);
  return labels;
}
// Labels points of a scatter chart. Each label ({ px, py, text }, placed in the given order) takes the candidate spot beside its
// dot that covers the fewest other labels, line segments ([[x1, y1], [x2, y2]]) and dots ([x, y]) inside box; a label moved
// away from its dot gets a leader line.
export function placeLabels(s, labels, dots, segs, box) {
  const placed = [];
  const overlaps = (a, b) => a.x0 < b.x1 && b.x0 < a.x1 && a.y0 < b.y1 && b.y0 < a.y1;
  const crosses = (b, [[ax, ay], [bx, by]]) => overlaps(b, { x0: Math.min(ax, bx) - 0.5, x1: Math.max(ax, bx) + 0.5, y0: Math.min(ay, by) - 0.5, y1: Math.max(ay, by) + 0.5 });
  for (const { px, py, text } of labels) {
    const t = s.append("text").attr("class", "t-ink").style("font-size", "10.5px").text(text);
    const tw = t.node().getComputedTextLength();
    const spots = [];
    for (const dy of [0, -13, 13, -26, 26, -39, 39]) {
      spots.push({ x0: px + 7, cy: py + dy, dy }, { x0: px - 7 - tw, cy: py + dy, dy });
      if (dy === 0) spots.push({ x0: px - tw / 2, cy: py - 12, dy: 0 }, { x0: px - tw / 2, cy: py + 13, dy: 0 });
    }
    let best = null;
    for (const sp of spots) {
      const b = { x0: sp.x0 - 1, x1: sp.x0 + tw + 1, y0: sp.cy - 6, y1: sp.cy + 6 };
      const out = b.x0 < box.x0 || b.x1 > box.x1 || b.y0 < box.y0 || b.y1 > box.y1;
      const score = (out ? 1e6 : 0) + 1000 * placed.filter(q => overlaps(q, b)).length + 40 * segs.filter(g => crosses(b, g)).length +
        10 * dots.filter(([dx, dy]) => dx > b.x0 - 3 && dx < b.x1 + 3 && dy > b.y0 - 3 && dy < b.y1 + 3).length + Math.abs(sp.dy) + (sp.x0 < px ? 2 : 0);
      if (!best || score < best.score) best = { ...sp, b, score };
    }
    placed.push(best.b);
    t.attr("x", best.x0).attr("y", best.cy + 4);
    if (Math.abs(best.dy) > 8) {
      const left = best.x0 < px;
      s.insert("line", ":scope > text").attr("x1", px + (left ? -3 : 3)).attr("y1", py).attr("x2", left ? best.b.x1 : best.b.x0).attr("y2", best.cy).style("stroke", "var(--v-muted)").style("stroke-width", 0.8);
    }
  }
}

// The series whose line passes nearest to (px, py), with that distance in pixels. Lines are straight between points.
export function nearestLine(series, px, py, X, Y) {
  const toSeg = (x1, y1, x2, y2) => {
    const dx = x2 - x1, dy = y2 - y1, l = dx * dx + dy * dy, t = l ? Math.max(0, Math.min(1, ((px - x1) * dx + (py - y1) * dy) / l)) : 0;
    return Math.hypot(px - x1 - t * dx, py - y1 - t * dy);
  };
  let z = null, d = Infinity;
  for (const s of series) {
    const p = s.pts.map(q => [X(q), Y(q)]);
    const ds = p.length === 1 ? toSeg(...p[0], ...p[0]) : Math.min(...d3.pairs(p).map(([a, b]) => toSeg(...a, ...b)));
    if (ds < d) { d = ds; z = s; }
  }
  return { z, d };
}
// How close the pointer must be to a line to pick it; a finger covers more than a mouse pointer.
export const pickRadius = ev => ev.pointerType === "touch" ? 24 : 12;

// ---------------------------------------------------------------- expanded chart

export const EXPAND_ICON = `<svg viewBox="0 0 16 16" width="14" height="14" aria-hidden="true"><path d="M2 6V2h4M10 2h4v4M14 10v4h-4M6 14H2v-4" fill="none" stroke="currentColor" stroke-width="1.6"/></svg>`;
// A chart enlarged to most of the screen. draw(host, width, height) fills host, and runs again when the dialog changes size.
// A modal dialog sits above every other element, so the tooltip moves into it while it is open.
export function chartDialog(title, draw, tools = "") {
  document.querySelector("dialog.big")?.close();
  const dlg = document.createElement("dialog");
  dlg.className = "big";
  dlg.setAttribute("aria-label", title);
  dlg.innerHTML = `<div class="big-head"><b>${esc(title)}</b><button type="button" class="big-x" aria-label="Close">×</button>${tools ? `<div class="big-tools">${tools}</div>` : ""}</div><div class="big-body"></div>`;
  // The dialog itself takes the focus, so that the close button does not open with a focus ring; Tab reaches it.
  dlg.tabIndex = -1;
  document.body.append(dlg);
  const body = dlg.querySelector(".big-body");
  let w = 0, h = 0;
  const redraw = () => {
    const cs = getComputedStyle(body);
    hideTip();
    body.replaceChildren();
    draw(body, body.clientWidth - parseFloat(cs.paddingLeft) - parseFloat(cs.paddingRight), body.clientHeight - parseFloat(cs.paddingTop) - parseFloat(cs.paddingBottom));
  };
  const ro = new ResizeObserver(() => { if (body.clientWidth !== w || body.clientHeight !== h) { w = body.clientWidth; h = body.clientHeight; redraw(); } });
  dlg.addEventListener("close", () => { ro.disconnect(); hideTip(); document.body.append(tip()); dlg.remove(); });
  dlg.addEventListener("click", e => { if (e.target === dlg || e.target.closest(".big-x")) dlg.close(); });
  dlg.append(tip());
  dlg.showModal();
  dlg.focus();
  // The first drawing does not wait for the observer, which reports only on the next rendered frame.
  w = body.clientWidth; h = body.clientHeight;
  redraw();
  ro.observe(body);
  return { dlg, redraw };
}
