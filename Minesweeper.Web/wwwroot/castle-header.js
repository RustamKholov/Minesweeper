/* Castle of Glass — shared header for non-Blazor (React/Vue/vanilla) apps.
 * Drop one tag into your page and you get the ecosystem chrome (brand → portal,
 * app-switcher from the live catalog, sign out):
 *
 *   <script src="/castle-header.js"
 *           data-app-name="My App"
 *           data-portal="https://rh-cloud.org/portal"
 *           data-auth="https://auth.rh-cloud.org"
 *           data-catalog="https://rh-cloud.org/apps.json"></script>
 *
 * Self-contained: injects its own styles and a sticky top bar. Sign-out goes to the
 * central gate, so logging out here logs you out everywhere.
 */
(function () {
  var cfg = (document.currentScript && document.currentScript.dataset) || {};
  var appName = cfg.appName || "";
  var portal = cfg.portal || "https://rh-cloud.org/portal";
  var auth = (cfg.auth || "https://auth.rh-cloud.org").replace(/\/$/, "");
  var catalog = cfg.catalog || "https://rh-cloud.org/apps.json";
  var signOut = auth + "/oauth2/sign_out?rd=" + encodeURIComponent("https://rh-cloud.org/");

  var CSS =
    '.cog-h{position:sticky;top:0;z-index:1000;display:flex;align-items:center;gap:16px;' +
    'padding:12px clamp(16px,4vw,40px);font-family:"Hanken Grotesk",system-ui,-apple-system,sans-serif;' +
    'background:rgba(243,246,250,.82);-webkit-backdrop-filter:blur(18px) saturate(1.3);' +
    'backdrop-filter:blur(18px) saturate(1.3);border-bottom:1px solid rgba(33,46,66,.08)}' +
    '.cog-h a{text-decoration:none}' +
    '.cog-h-brand{display:flex;align-items:center;gap:9px;color:#1E2530;font-weight:600;font-size:15px}' +
    '.cog-h-mk{width:30px;height:30px;border-radius:9px;display:flex;align-items:center;justify-content:center;' +
    'background:linear-gradient(160deg,#fff,#E7EEF7);border:1px solid rgba(255,255,255,.7);color:#3D5A80}' +
    '.cog-h-mk svg{width:17px;height:17px}' +
    '.cog-h-sep{color:#8C97A6;font-weight:400}.cog-h-app{color:#2C415E;font-weight:600}' +
    '.cog-h-right{margin-left:auto;display:flex;align-items:center;gap:10px;position:relative}' +
    '.cog-h-btn{cursor:pointer;display:inline-flex;align-items:center;justify-content:center;width:38px;height:38px;' +
    'border-radius:10px;color:#8C97A6;border:1px solid transparent;background:none}' +
    '.cog-h-btn:hover{background:rgba(255,255,255,.6);color:#3D5A80}.cog-h-btn svg{width:18px;height:18px}' +
    '.cog-h-menu{position:absolute;right:0;top:46px;min-width:220px;padding:10px;border-radius:16px;display:none;' +
    'background:rgba(255,255,255,.92);-webkit-backdrop-filter:blur(22px);backdrop-filter:blur(22px);' +
    'border:1px solid rgba(255,255,255,.7);box-shadow:0 24px 50px -24px rgba(28,40,60,.5)}' +
    '.cog-h-menu.open{display:block}' +
    '.cog-h-menu .h{font-family:"JetBrains Mono",ui-monospace,monospace;font-size:10.5px;letter-spacing:.14em;' +
    'text-transform:uppercase;color:#8C97A6;padding:4px 10px 8px}' +
    '.cog-h-item{display:flex;align-items:center;gap:10px;padding:9px 10px;border-radius:10px;font-size:14px;' +
    'font-weight:500;color:#1E2530}.cog-h-item:hover{background:#EAF0F8}' +
    '.cog-h-dot{width:8px;height:8px;border-radius:3px;background:linear-gradient(150deg,#3D5A80,#2C415E)}' +
    '.cog-h-out{margin-top:6px;padding-top:10px;border-top:1px solid rgba(33,46,66,.08);color:#27405E}';

  var castleSvg =
    '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linejoin="round" stroke-linecap="round">' +
    '<path d="M4 21V10l2-1.4V6l2 .9V8l2-1.6V6l2 .9V8l2-1.6V6l2 .9v.5L20 10v11"/><path d="M4 21h16"/>' +
    '<path d="M10 21v-4a2 2 0 0 1 4 0v4"/></svg>';
  var gridSvg =
    '<svg viewBox="0 0 24 24" fill="currentColor"><circle cx="5" cy="5" r="1.8"/><circle cx="12" cy="5" r="1.8"/>' +
    '<circle cx="19" cy="5" r="1.8"/><circle cx="5" cy="12" r="1.8"/><circle cx="12" cy="12" r="1.8"/>' +
    '<circle cx="19" cy="12" r="1.8"/><circle cx="5" cy="19" r="1.8"/><circle cx="12" cy="19" r="1.8"/>' +
    '<circle cx="19" cy="19" r="1.8"/></svg>';

  function el(html) { var t = document.createElement("template"); t.innerHTML = html.trim(); return t.content.firstChild; }

  function build() {
    var style = document.createElement("style"); style.textContent = CSS; document.head.appendChild(style);

    var header = el(
      '<header class="cog-h">' +
        '<a class="cog-h-brand" href="' + portal + '"><span class="cog-h-mk">' + castleSvg + '</span>' +
          '<span>Castle of Glass</span>' +
          (appName ? '<span class="cog-h-sep">·</span><span class="cog-h-app">' + appName + '</span>' : "") +
        '</a>' +
        '<div class="cog-h-right">' +
          '<button class="cog-h-btn" id="cog-h-toggle" aria-label="Switch apps">' + gridSvg + '</button>' +
          '<div class="cog-h-menu" id="cog-h-menu"><div class="h">Your apps</div>' +
            '<div id="cog-h-apps"></div>' +
            '<a class="cog-h-item cog-h-out" href="' + signOut + '">Sign out</a>' +
          '</div>' +
        '</div>' +
      '</header>');
    document.body.insertBefore(header, document.body.firstChild);

    var menu = header.querySelector("#cog-h-menu");
    header.querySelector("#cog-h-toggle").addEventListener("click", function (e) {
      e.stopPropagation(); menu.classList.toggle("open");
    });
    document.addEventListener("click", function () { menu.classList.remove("open"); });

    fetch(catalog).then(function (r) { return r.json(); }).then(function (apps) {
      var box = header.querySelector("#cog-h-apps");
      (apps || []).forEach(function (a) {
        var item = el('<a class="cog-h-item" href="' + a.url + '"><span class="cog-h-dot"></span>' + a.name + "</a>");
        box.appendChild(item);
      });
      var home = el('<a class="cog-h-item" href="' + portal + '"><span class="cog-h-dot"></span>Portal home</a>');
      box.appendChild(home);
    }).catch(function () { /* catalog unreachable — switcher just shows Portal home */
      header.querySelector("#cog-h-apps").appendChild(
        el('<a class="cog-h-item" href="' + portal + '"><span class="cog-h-dot"></span>Portal home</a>'));
    });
  }

  if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", build);
  else build();
})();
