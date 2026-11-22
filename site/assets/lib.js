// Shared data access, formatting and chart helpers for the benchmark site.
// d3 is loaded as a global UMD script by index.html.
const d3 = globalThis.d3;
export { d3 };

export const PATTERNS = ["Random", "SingleElementMoved", "Sorted", "Reversed", "PipeOrgan", "ManyDuplicates"];
export const PATTERN_SHORT = { Random: "Random", SingleElementMoved: "1-Moved", Sorted: "Sorted", Reversed: "Reversed", PipeOrgan: "PipeOrgan", ManyDuplicates: "ManyDup" };
const CATEGORY_ORDER = ["Exchange", "Selection", "SelectionSimd", "Insertion", "Merge", "MergeSlow", "Heap", "Partition", "Distribution", "Network", "Tree"];
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

// Sizes measured by at least two categories, largest first; every size when no size qualifies.
export function leagueSizes(run) {
  const shared = run.allSizes.filter(n => run.categories.filter(c => run.sizesOf(c).includes(n)).length >= 2);
  return (shared.length ? shared : run.allSizes).slice().reverse();
}

// Anchor-based correction: scales a category's times so its reference run matches the reference run of `base`.
export function anchorFactor(run, c, n, p, base) {
  if (c === base) return 1;
  const a = run.get(base, run.reference, n, p), b = run.get(c, run.reference, n, p);
  return a && b ? a.mean / b.mean : null;
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
export const fmtX = r => "×" + (r < 10 ? r.toFixed(2) : r < 100 ? r.toFixed(1) : Math.round(r));
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

// ---------------------------------------------------------------- controls

export function selectEl(id, label, options, value) {
  return `<label for="${id}">${esc(label)}<select id="${id}">${options.map(o => {
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
export function nudge(labels, gap = 12) {
  labels.sort((a, b) => a.y - b.y);
  for (let i = 1; i < labels.length; i++) if (labels[i].y - labels[i - 1].y < gap) labels[i].y = labels[i - 1].y + gap;
  return labels;
}
