const { chromium } = require('playwright');
const sleep=ms=>new Promise(r=>setTimeout(r,ms));
(async()=>{
  const b=await chromium.launch({headless:true});
  const ctx=await b.newContext({viewport:{width:390,height:844},deviceScaleFactor:2,isMobile:true,hasTouch:true});
  const p=await ctx.newPage();
  await p.goto('http://localhost:5027/',{waitUntil:'domcontentloaded'});
  await p.waitForSelector('.board-grid .cell',{timeout:60000}); await sleep(700);
  const g=p.getByRole('button',{name:"Got it, let's dig"}); if(await g.count()){await g.click();await sleep(400);}
  // open settings and check a control is actually styled
  const btns=p.locator('.app-header-actions .help-button');
  await btns.nth(await btns.count()-2).click(); await sleep(300);
  const styled=await p.evaluate(()=>{
    const sw=document.querySelector('.switch'); const seg=document.querySelector('.seg');
    if(!sw||!seg) return {found:false};
    const s=getComputedStyle(sw); const g=getComputedStyle(seg);
    return {found:true, switchW:s.width, switchRadius:s.borderRadius, segRadius:g.borderRadius};
  });
  console.log('settings control styled:', JSON.stringify(styled));
  // close, then fullscreen check
  await p.keyboard.press('Escape'); await sleep(200);
  await p.locator('.modal-scrim').count().then(async c=>{ if(c) { /* click done */ }});
  // click Done if modal still open
  const done=p.locator('.settings-footer .primary-button'); if(await done.count()){await done.click();await sleep(200);}
  await btns.first().click(); await sleep(800); // fullscreen toggle (first action button)
  const fs=await p.evaluate(()=>({cls:document.documentElement.classList.contains('ms-fullscreen'),cog:getComputedStyle(document.querySelector('.cog-h')).display,hudBg:getComputedStyle(document.querySelector('.game-hud')).backgroundColor}));
  console.log('fullscreen:', JSON.stringify(fs));
  await b.close();
})().catch(e=>{console.error('ERR',e);process.exit(1);});
