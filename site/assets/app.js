import { loadIndex, loadRun, esc, fmtDate, hideTip } from "./lib.js";
import * as explorer from "./views/explorer.js";
import * as leagues from "./views/leagues.js";
import * as complexity from "./views/complexity.js";
import * as stability from "./views/stability.js";
import * as historyView from "./views/history.js";

const VIEWS = [
  ["explorer", "Explorer", explorer],
  ["leagues", "Leagues", leagues],
  ["complexity", "Complexity", complexity],
  ["stability", "Stability", stability],
  ["history", "History", historyView],
];

const root = document.getElementById("view");
let index = null;

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
  hideTip();
  const { route, params } = parseHash();
  renderNav(route, params);
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
  document.getElementById("meta").innerHTML = [
    esc(fmtDate(entry.timestamp)), esc(entry.cpu), `${entry.cases.toLocaleString("en-US")} cases`,
    entry.runUrl ? `<a href="${esc(entry.runUrl)}">workflow run</a>` : null,
    entry.source === "readme" ? "imported from README history (no per-iteration data)" : null,
  ].filter(Boolean).join(" · ");

  const run = await loadRun(entry);
  const view = VIEWS.find(v => v[0] === route)[2];
  const current = { ...params };
  root.innerHTML = "";
  await view.render(root, {
    run, index, params,
    setParams: p => { Object.assign(current, p); writeHash(route, current); },
  });
}

window.addEventListener("hashchange", render);
render().catch(err => {
  root.innerHTML = `<div class="panel empty">Failed to load benchmark data: ${esc(err.message)}</div>`;
  console.error(err);
});
