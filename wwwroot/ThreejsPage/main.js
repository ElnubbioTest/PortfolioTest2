import { nextHex } from "./colourhandler.js";
let globalCounter = 0;
const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(75, 800 / 600, 0.1, 1000);
const renderer = new THREE.WebGLRenderer();
renderer.setSize(800, 600);
// document.body.appendChild(renderer.domElement);
document.querySelector("#container").appendChild(renderer.domElement);
camera.position.z = 5;

//

//Container Outline
const containerOutlineGeometry = new THREE.BoxGeometry(7.3, 7.3, 0);
const containerOutlineMaterial = new THREE.MeshBasicMaterial({ color: 0xffffff });
const containerOutline = new THREE.Mesh(containerOutlineGeometry, containerOutlineMaterial);
containerOutline.position.z = -0.002;
scene.add(containerOutline);
//Container
const containerGeometry = new THREE.BoxGeometry(7, 7, 0);
const containerMaterial = new THREE.MeshBasicMaterial({ color: 0x000000 });
const container = new THREE.Mesh(containerGeometry, containerMaterial);
container.position.z = -0.002;
scene.add(container);

//Square 1
// const square1Geometry = new THREE.BoxGeometry(1, 1, 0);
// const square1Material = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
// const square1 = new THREE.Mesh(square1Geometry, square1Material);
// scene.add(square1);

//Square 2
const square2Geometry = new THREE.BoxGeometry(1, 1, 0);
const square2Material = new THREE.MeshBasicMaterial({ color: 0x0000ff });
const square2 = new THREE.Mesh(square2Geometry, square2Material);
let square2SpeedMult = 0.05;
let square2Speed = { x: square2SpeedMult, y: square2SpeedMult + 0.01 };
square2.position.x = 2;
square2.position.y = 2.5;
let square2TrailCurrentHex = "#0000ff";
scene.add(square2);

//Animation loop
function animate() {
    requestAnimationFrame(animate);
    square2.position.x += square2Speed.x;
    square2.position.y += square2Speed.y;
    handleContainerCollision();
    createSquareTrail(square2, 0xff0000);
    renderer.render(scene, camera);
}
animate();

//Bounce
function handleContainerCollision() {
    if (square2.position.x > 3 || square2.position.x < -3) {
        square2Speed.x *= -1;
    }
    if (square2.position.y > 3 || square2.position.y < -3) {
        square2Speed.y *= -1;
    }
}

//Create a new square behind the current position of a given square
function createSquareTrail(square, colour) {
    let trailGeometry = new THREE.BoxGeometry(1, 1, 0);
    let trailMaterial = new THREE.MeshBasicMaterial({ color: colour });
    let newSquare = new THREE.Mesh(trailGeometry, trailMaterial);
    newSquare.position.x = square.position.x;
    newSquare.position.y = square.position.y;
    newSquare.position.z = -0.001;
    newSquare.name = `square-${globalCounter}`;
    square2TrailCurrentHex = nextHex(square2TrailCurrentHex);
    newSquare.material.color.setHex(square2TrailCurrentHex);

    scene.add(newSquare);
    globalCounter++;
    if (globalCounter > 1000) {
        let a = scene.getObjectByName(`square-${globalCounter - 1001}`);
        scene.remove(a);
    }
}
