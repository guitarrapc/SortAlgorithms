import { loadIndex, loadRun, esc, fmtDate, hideTip } from "./lib.js";
import * as explorer from "./views/explorer.js";
import * as leagues from "./views/leagues.js";
import * as compare from "./views/compare.js";
import * as complexity from "./views/complexity.js";
import * as stability from "./views/stability.js";
import * as historyView from "./views/history.js";

const VIEWS = [
  ["explorer", "Explorer", explorer],
  ["leagues", "Leagues", leagues],
  ["compare", "Compare", compare],
  ["complexity", "Complexity", complexity],
  ["stability", "Stability", stability],
  ["history", "History", historyView],
];

const root = document.getElementById("view");
let index = null;
let onResize = null;

function parseHash() {
  const [route, qs] = location.hash.slice(1).split("?");
  return {
    route: VIEWS.some(v => v[0] === route) ? route : "explorer",
    params: Object.fromEntries(new URLSearchParams(qs || "")),
  };
}

function writeHash(route, params) {
  const qs = new URLSearchParams(Object.entries(params).filter(([, v]) => v !== undefined && v !== null && v !== "")).toString();
  history.replaceState(null, "", "#" + route + (qs ? "?" + qs : ""));
}

function renderNav(route, params) {
  const keep = params.run ? "?run=" + encodeURIComponent(params.run) : "";
  document.getElementById("nav").innerHTML = VIEWS.map(([id, label]) =>
    `<a href="#${id}${keep}"${id === route ? ' aria-current="page"' : ""}>${label}</a>`).join("");
}

// Sticky headers inside a view sit just below the sticky tabs, whose height differs between phone and desktop.
// Set when the tabs are rendered, since a ResizeObserver first reports only on the next rendered frame.
function setTabsHeight() {
  document.documentElement.style.setProperty("--tabs-h", document.querySelector(".tabs").offsetHeight + "px");
}

function renderRunPicker(entry, route, params) {
  const runs = [...index.runs].reverse();
  const host = document.getElementById("run-pick");
  host.innerHTML = `<label for="run-select">Run <select id="run-select">${runs.map(r =>
    `<option value="${esc(r.id)}"${r.id === entry.id ? " selected" : ""}>${esc(fmtDate(r.timestamp))} · ${esc(r.cpu)}${r.source === "readme" ? " (imported)" : ""}</option>`).join("")}</select></label>`;
  host.querySelector("select").addEventListener("change", e => {
    const next = { ...params, run: e.target.value === index.latest ? undefined : e.target.value };
    location.hash = "#" + route + (Object.keys(next).some(k => next[k]) ? "?" + new URLSearchParams(Object.entries(next).filter(([, v]) => v)).toString() : "");
  });
}

async function render() {
  // An enlarged chart belongs to the view it was opened from.
  document.querySelector("dialog.big")?.close();
  hideTip();
  onResize = null;
  const { route, params } = parseHash();
  renderNav(route, params);
  setTabsHeight();
  if (!index) {
    try {
      index = await loadIndex();
    } catch {
      root.innerHTML = `<div class="panel empty">No benchmark data has been published yet. Run the Benchmark workflow with all benchmarks selected.</div>`;
      document.getElementById("meta").textContent = "";
      return;
    }
  }
  const entry = index.runs.find(r => r.id === params.run) || index.runs.find(r => r.id === index.latest) || index.runs.at(-1);
  if (!entry) {
    root.innerHTML = `<div class="panel empty">The run index is empty.</div>`;
    return;
  }
  renderRunPicker(entry, route, params);
  // The run picker already shows the date and CPU.
  document.getElementById("meta").innerHTML = [
    `<span class="sm-hide">${entry.cases.toLocaleString("en-US")} cases</span>`,
    entry.runUrl ? `<a href="${esc(entry.runUrl)}">workflow run</a>` : null,
    entry.source === "readme" ? `<span class="sm-hide">imported from README history (no per-iteration data)</span>` : null,
  ].filter(Boolean).join(`<span class="sm-hide"> · </span>`);

  const run = await loadRun(entry);
  const view = VIEWS.find(v => v[0] === route)[2];
  const current = { ...params };
  root.innerHTML = "";
  await view.render(root, {
    run, index, params,
    setParams: p => { Object.assign(current, p); writeHash(route, current); },
    // Views register the charts that depend on the container width.
    onResize: fn => { onResize = fn; },
  });
}

// Redraw width-dependent charts when the content width changes; height-only changes (mobile URL bar) are ignored.
// ResizeObserver also sees changes that fire no window resize event.
const contentWidth = el => { const cs = getComputedStyle(el); return Math.round(el.clientWidth - parseFloat(cs.paddingLeft) - parseFloat(cs.paddingRight)); };
let lastWidth = contentWidth(root), resizeTimer = 0;
new ResizeObserver(entries => {
  const width = Math.round(entries[0].contentRect.width);
  if (width === lastWidth) return;
  lastWidth = width;
  clearTimeout(resizeTimer);
  resizeTimer = setTimeout(() => { hideTip(); onResize?.(); }, 150);
}).observe(root);
new ResizeObserver(setTabsHeight).observe(document.querySelector(".tabs"));
// Touch has no pointerleave after a tap: a tap outside a chart or a scroll closes the tooltip.
document.addEventListener("pointerdown", e => { if (!e.target.closest("svg.chart")) hideTip(); });
window.addEventListener("scroll", hideTip, { passive: true });

window.addEventListener("hashchange", render);
render().catch(err => {
  root.innerHTML = `<div class="panel empty">Failed to load benchmark data: ${esc(err.message)}</div>`;
  console.error(err);
});
