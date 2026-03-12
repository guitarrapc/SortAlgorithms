'use strict';
// Video recording for the sort visualization content area.
//
// Captures each sort-card (header + canvas + stats) by compositing them onto
// an offscreen canvas, then encodes the result into a WebM via MediaRecorder.
//
// Capture target: .visualization-content (sort cards only, no control bar / seek bar)
//
// Flow:
//   1. startRecording(selector, fps) — begins periodic sort-card compositing.
//   2. stopRecording(filename)       — stops and triggers a .webm download.

window.videoRecorder = {
  _mediaRecorder: null,
  _chunks: [],
  _offscreen: null,
  _stream: null,
  _recording: false,
  _captureInterval: null,

  /**
   * Start recording the specified DOM element.
   * The element should contain .sort-card elements with canvas visualizations.
   * @param {string} selector - CSS selector for the content area (e.g. '.visualization-content')
   * @param {number} fps      - Frames per second (default: 30)
   * @returns {boolean} true if recording started successfully
   */
  startRecording: function (selector, fps) {
    if (this._recording) return false;

    const target = document.querySelector(selector);
    if (!target) {
      console.error('[videoRecorder] Element not found:', selector);
      return false;
    }

    fps = fps || 30;

    const canvases = target.querySelectorAll('canvas');
    if (canvases.length === 0) {
      console.error('[videoRecorder] No canvas elements found in:', selector);
      return false;
    }

    // Use the target element's bounding rect for video dimensions
    const rect = target.getBoundingClientRect();
    const width = Math.round(rect.width);
    const height = Math.round(rect.height);

    const offscreen = document.createElement('canvas');
    offscreen.width = width;
    offscreen.height = height;
    this._offscreen = offscreen;

    const ctx = offscreen.getContext('2d');

    const stream = offscreen.captureStream(fps);
    this._stream = stream;

    // Codec selection: VP9 > VP8 > default
    const mimeType = MediaRecorder.isTypeSupported('video/webm;codecs=vp9')
      ? 'video/webm;codecs=vp9'
      : MediaRecorder.isTypeSupported('video/webm;codecs=vp8')
        ? 'video/webm;codecs=vp8'
        : 'video/webm';

    this._chunks = [];
    const recorder = new MediaRecorder(stream, {
      mimeType: mimeType,
      videoBitsPerSecond: 8_000_000,
    });
    recorder.ondataavailable = (e) => {
      if (e.data && e.data.size > 0) this._chunks.push(e.data);
    };
    this._mediaRecorder = recorder;
    recorder.start(100);

    // Periodic compositing: paint each sort-card's elements onto the offscreen canvas
    const intervalMs = 1000 / fps;
    this._captureInterval = setInterval(() => {
      const targetRect = target.getBoundingClientRect();

      // Background
      ctx.fillStyle = '#0a0a1a';
      ctx.fillRect(0, 0, offscreen.width, offscreen.height);

      // Paint each sort-card: header background, card border, completion glow
      for (const card of target.querySelectorAll('.sort-card')) {
        const cardRect = card.getBoundingClientRect();
        const cx = cardRect.left - targetRect.left;
        const cy = cardRect.top - targetRect.top;
        const cw = cardRect.width;
        const ch = cardRect.height;

        // Card background
        ctx.fillStyle = '#1a1a1a';
        ctx.fillRect(cx, cy, cw, ch);

        // Card border (green glow if completed)
        const isCompleted = card.classList.contains('sort-card--completed');
        ctx.strokeStyle = isCompleted ? '#10B981' : '#333';
        ctx.lineWidth = 2;
        ctx.strokeRect(cx, cy, cw, ch);

        // Header background
        const header = card.querySelector('.sort-card__header');
        if (header) {
          const hRect = header.getBoundingClientRect();
          ctx.fillStyle = '#252525';
          ctx.fillRect(
            hRect.left - targetRect.left,
            hRect.top - targetRect.top,
            hRect.width,
            hRect.height
          );
        }

        // Stats summary background
        const stats = card.querySelector('.sort-stats-summary');
        if (stats) {
          const sRect = stats.getBoundingClientRect();
          ctx.fillStyle = '#242a27';
          ctx.fillRect(
            sRect.left - targetRect.left,
            sRect.top - targetRect.top,
            sRect.width,
            sRect.height
          );
        }
      }

      // Canvas elements (the actual sort visualization)
      for (const canvas of target.querySelectorAll('canvas')) {
        if (canvas.width === 0 || canvas.height === 0) continue;
        const cRect = canvas.getBoundingClientRect();
        try {
          ctx.drawImage(
            canvas,
            cRect.left - targetRect.left,
            cRect.top - targetRect.top,
            cRect.width,
            cRect.height
          );
        } catch (_) {
          // Ignore tainted canvas errors (e.g. picture mode with cross-origin images)
        }
      }

      // Algorithm name labels
      // .sort-card__algorithm-name: N=1 text span
      // .sort-card__algo-select:    N>1 <select> element (read selectedOptions text)
      for (const card of target.querySelectorAll('.sort-card')) {
        const header = card.querySelector('.sort-card__header');
        if (!header) continue;

        let algoName = '';
        const nameSpan = header.querySelector('.sort-card__algorithm-name');
        if (nameSpan) {
          algoName = nameSpan.textContent.trim();
        } else {
          const sel = header.querySelector('.sort-card__algo-select');
          if (sel && sel.selectedOptions.length > 0) {
            algoName = sel.selectedOptions[0].textContent.trim();
          }
        }

        if (algoName) {
          const hRect = header.getBoundingClientRect();
          ctx.font = 'bold 14px system-ui, -apple-system, sans-serif';
          ctx.fillStyle = '#e5e7eb';
          ctx.textBaseline = 'middle';
          // Draw left-aligned inside the header, after the drag handle area
          ctx.fillText(
            algoName,
            hRect.left - targetRect.left + 30, // offset past drag handle
            hRect.top - targetRect.top + hRect.height / 2
          );
        }

        // Complexity badge
        const badge = header.querySelector('.complexity-badge');
        if (badge) {
          const bRect = badge.getBoundingClientRect();
          ctx.font = '11px system-ui, -apple-system, sans-serif';
          ctx.fillStyle = '#c8aa6e';
          ctx.textBaseline = 'middle';
          ctx.textAlign = 'center';
          ctx.fillText(
            badge.textContent.trim(),
            bRect.left - targetRect.left + bRect.width / 2,
            bRect.top - targetRect.top + bRect.height / 2
          );
          ctx.textAlign = 'start';
        }
      }

      // Stats values (each .stat-mini contains .value and .label)
      for (const statMini of target.querySelectorAll('.stat-mini')) {
        const miniRect = statMini.getBoundingClientRect();
        const mx = miniRect.left - targetRect.left;
        const my = miniRect.top - targetRect.top;
        const mw = miniRect.width;
        const mh = miniRect.height;

        // stat-mini background
        ctx.fillStyle = 'rgba(127, 168, 111, 0.08)';
        ctx.fillRect(mx, my, mw, mh);
        ctx.strokeStyle = 'rgba(127, 168, 111, 0.2)';
        ctx.lineWidth = 1;
        ctx.strokeRect(mx, my, mw, mh);

        // Value (top)
        const valueEl = statMini.querySelector('.value');
        if (valueEl) {
          ctx.font = 'bold 13px Consolas, Monaco, "Courier New", monospace';
          ctx.fillStyle = '#e5e7eb';
          ctx.textBaseline = 'middle';
          ctx.textAlign = 'center';
          ctx.fillText(
            valueEl.textContent.trim(),
            mx + mw / 2,
            my + mh * 0.38
          );
        }

        // Label (bottom)
        const labelEl = statMini.querySelector('.label');
        if (labelEl) {
          ctx.font = '10px system-ui, -apple-system, sans-serif';
          ctx.fillStyle = '#9ca3af';
          ctx.textBaseline = 'middle';
          ctx.textAlign = 'center';
          ctx.fillText(
            labelEl.textContent.trim().toUpperCase(),
            mx + mw / 2,
            my + mh * 0.72
          );
        }
        ctx.textAlign = 'start';
      }
    }, intervalMs);

    this._recording = true;
    return true;
  },

  /**
   * Stop recording and download the video as a .webm file.
   * @param {string} filename - The download filename (without extension)
   */
  stopRecording: function (filename) {
    if (!this._recording || !this._mediaRecorder) return;

    if (this._captureInterval) {
      clearInterval(this._captureInterval);
      this._captureInterval = null;
    }

    const recorder = this._mediaRecorder;
    const chunks = this._chunks;

    recorder.onstop = () => {
      const blob = new Blob(chunks, { type: recorder.mimeType });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = (filename || 'sortvivo-recording') + '.webm';
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);

      this._chunks = [];
      this._mediaRecorder = null;
      if (this._stream) {
        this._stream.getTracks().forEach(t => t.stop());
        this._stream = null;
      }
      this._offscreen = null;
    };

    recorder.stop();
    this._recording = false;
  },

  /**
   * Check if currently recording.
   * @returns {boolean}
   */
  isRecording: function () {
    return this._recording;
  },
};
