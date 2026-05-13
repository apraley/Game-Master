const canvas = document.getElementById('game');
const ctx = canvas.getContext('2d');
const TILE = 32;
const W = canvas.width / TILE;
const H = canvas.height / TILE;

const ui = {
  health: document.getElementById('health'),
  coffee: document.getElementById('coffee'),
  pie: document.getElementById('pie'),
  room: document.getElementById('room'),
  clues: document.getElementById('clues'),
  messages: document.getElementById('messages')
};

const keys = new Set();
addEventListener('keydown', (e) => {
  keys.add(e.key.toLowerCase());
  if ([" ", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"].includes(e.key)) e.preventDefault();
});
addEventListener('keyup', (e) => keys.delete(e.key.toLowerCase()));

const player = {
  x: 2, y: 2, hp: 6, maxHp: 6, coffee: 0, pie: 0, clues: 0,
  face: 'down', attackCd: 0, invuln: 0
};

const maps = [
  {
    name: 'Sheriff Station',
    palette: ['#1f2a44', '#2f436a'],
    walls: [[0,0,16,1],[0,0,1,12],[15,0,1,12],[0,11,16,1],[6,4,4,1],[3,7,1,3],[12,2,1,4]],
    items: [{x:13,y:9,type:'coffee'},{x:2,y:9,type:'clue'}],
    enemies: [{x:10,y:8,type:'woodsman',hp:2}]
  },
  {
    name: 'Ghostwood Forest',
    palette: ['#16351d', '#295c33'],
    walls: [[0,0,16,1],[0,0,1,12],[15,0,1,12],[0,11,16,1],[4,2,1,7],[8,1,1,4],[11,5,1,6]],
    items: [{x:2,y:2,type:'pie'},{x:14,y:9,type:'clue'}],
    enemies: [{x:13,y:3,type:'bob',hp:3},{x:6,y:9,type:'woodsman',hp:2}]
  },
  {
    name: 'Fire Walk With Me Corridor',
    palette: ['#3a0f17', '#601a2a'],
    walls: [[0,0,16,1],[0,0,1,12],[15,0,1,12],[0,11,16,1],[2,3,12,1],[2,7,12,1],[7,3,1,5]],
    items: [{x:3,y:9,type:'coffee'}],
    enemies: [{x:12,y:9,type:'doppel',hp:3},{x:4,y:1,type:'woodsman',hp:2}]
  },
  {
    name: 'The Return: Glass Box',
    palette: ['#202020', '#444'],
    walls: [[0,0,16,1],[0,0,1,12],[15,0,1,12],[0,11,16,1],[3,2,10,1],[3,8,10,1],[3,3,1,5],[12,3,1,5]],
    items: [{x:14,y:2,type:'pie'},{x:2,y:10,type:'clue'}],
    enemies: [{x:8,y:5,type:'experiment',hp:4}]
  },
  {
    name: 'Black Lodge / Red Room',
    palette: ['#2b0008', '#650012'],
    walls: [[0,0,16,1],[0,0,1,12],[15,0,1,12],[0,11,16,1],[5,2,1,7],[10,2,1,7]],
    items: [],
    enemies: [{x:8,y:5,type:'judy',hp:6}]
  }
];

let roomIndex = 0;
let mapState = structuredClone(maps);
let win = false;

function addMessage(text) {
  const li = document.createElement('li');
  li.textContent = text;
  ui.messages.prepend(li);
  while (ui.messages.children.length > 7) ui.messages.removeChild(ui.messages.lastChild);
}

function collides(x, y) {
  if (x < 1 || y < 1 || x > W-2 || y > H-2) return true;
  const m = mapState[roomIndex];
  return m.walls.some(([wx,wy,ww,wh]) => x >= wx && x < wx+ww && y >= wy && y < wy+wh);
}

function moveEnemy(enemy) {
  if (Math.random() < 0.3) return;
  const dirs = [[1,0],[-1,0],[0,1],[0,-1]];
  dirs.sort(() => Math.random() - .5);
  for (const [dx,dy] of dirs) {
    const nx = enemy.x + dx, ny = enemy.y + dy;
    if (!collides(nx, ny) && !(nx===player.x && ny===player.y)) { enemy.x=nx; enemy.y=ny; break; }
  }
}

function attack() {
  if (player.attackCd > 0) return;
  player.attackCd = 15;
  let tx = player.x, ty = player.y;
  if (player.face==='up') ty--; if (player.face==='down') ty++;
  if (player.face==='left') tx--; if (player.face==='right') tx++;

  const enemies = mapState[roomIndex].enemies;
  const hit = enemies.find(e => e.x===tx && e.y===ty);
  if (hit) {
    hit.hp -= 2;
    addMessage(`Cooper strikes ${hit.type.toUpperCase()}!`);
    if (hit.hp <= 0) {
      enemies.splice(enemies.indexOf(hit),1);
      player.clues++;
      addMessage(`${hit.type} dispelled. A clue surfaces.`);
      if (hit.type === 'judy') { win = true; addMessage('The cycle breaks. You solved Twin Peaks.'); }
    }
  }
}

function interact() {
  const items = mapState[roomIndex].items;
  const found = items.find(i => Math.abs(i.x-player.x)+Math.abs(i.y-player.y)<=1);
  if (!found) return;
  if (found.type === 'coffee') { player.coffee++; player.hp = Math.min(player.maxHp, player.hp + 1); addMessage('Damn fine coffee restores 1 health.'); }
  if (found.type === 'pie') { player.pie++; addMessage('Cherry pie found. Morale rises.'); }
  if (found.type === 'clue') { player.clues++; addMessage('You found a crucial case clue.'); }
  items.splice(items.indexOf(found),1);
}

function update() {
  if (win || player.hp <= 0) return;
  let dx=0, dy=0;
  if (keys.has('arrowup') || keys.has('w')) { dy=-1; player.face='up'; }
  else if (keys.has('arrowdown') || keys.has('s')) { dy=1; player.face='down'; }
  else if (keys.has('arrowleft') || keys.has('a')) { dx=-1; player.face='left'; }
  else if (keys.has('arrowright') || keys.has('d')) { dx=1; player.face='right'; }

  if ((keys.has(' ') || keys.has('space')) && player.attackCd===0) attack();
  if (keys.has('e')) interact();

  if ((dx||dy) && !collides(player.x+dx, player.y+dy)) { player.x+=dx; player.y+=dy; }

  if (player.x===14 && player.y===10 && roomIndex < mapState.length-1 && mapState[roomIndex].enemies.length===0) {
    roomIndex++; player.x=1; player.y=1;
    addMessage(`Entering ${mapState[roomIndex].name}.`);
  }

  for (const enemy of mapState[roomIndex].enemies) {
    moveEnemy(enemy);
    if (enemy.x===player.x && enemy.y===player.y && player.invuln===0) {
      player.hp--; player.invuln=30;
      addMessage(`${enemy.type} hits Cooper!`);
    }
  }

  if (player.attackCd>0) player.attackCd--;
  if (player.invuln>0) player.invuln--;
}

function drawRect(x,y,w,h,c) { ctx.fillStyle=c; ctx.fillRect(x*TILE,y*TILE,w*TILE,h*TILE); }

function render() {
  const map = mapState[roomIndex];
  for (let y=0;y<H;y++) {
    for (let x=0;x<W;x++) {
      ctx.fillStyle = (x+y)%2 ? map.palette[0] : map.palette[1];
      ctx.fillRect(x*TILE,y*TILE,TILE,TILE);
    }
  }

  map.walls.forEach(([x,y,w,h]) => drawRect(x,y,w,h,'#111'));

  for (const item of map.items) {
    ctx.fillStyle = item.type==='coffee' ? '#6f4e37' : item.type==='pie' ? '#ffb347' : '#fff176';
    ctx.fillRect(item.x*TILE+8,item.y*TILE+8,16,16);
  }

  for (const e of map.enemies) {
    const color = ({woodsman:'#6b705c', bob:'#e63946', doppel:'#8d99ae', experiment:'#9d4edd', judy:'#ff006e'})[e.type] || '#fff';
    ctx.fillStyle = color;
    ctx.fillRect(e.x*TILE+4,e.y*TILE+4,24,24);
  }

  ctx.fillStyle = player.invuln%4<2 ? '#59c3c3' : '#bde0fe';
  ctx.fillRect(player.x*TILE+4,player.y*TILE+4,24,24);

  if (win || player.hp<=0) {
    ctx.fillStyle='rgba(0,0,0,.7)'; ctx.fillRect(0,0,canvas.width,canvas.height);
    ctx.fillStyle='#f4d35e'; ctx.font='28px monospace';
    ctx.fillText(win ? 'CASE CLOSED' : 'LOST IN THE LODGE', 110, 170);
    ctx.font='16px monospace';
    ctx.fillText('Refresh to play again.', 160, 210);
  }

  ui.health.textContent = player.hp;
  ui.coffee.textContent = player.coffee;
  ui.pie.textContent = player.pie;
  ui.clues.textContent = player.clues;
  ui.room.textContent = roomIndex + 1;
}

addMessage('Agent Dale Cooper enters Twin Peaks.');
addMessage('Defeat all enemies to open each room exit at bottom-right.');
addMessage('Find clues and face Judy in the Red Room.');

(function loop(){
  update();
  render();
  requestAnimationFrame(loop);
})();
