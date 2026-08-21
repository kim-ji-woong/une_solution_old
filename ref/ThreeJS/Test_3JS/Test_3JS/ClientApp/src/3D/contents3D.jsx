import React, { Component } from 'react';
import styles from './css/_3d.module.css';
import * as THREE from "three/build/three.module.js";
import Stats from "three/examples/jsm/libs/stats.module.js";
import { OrbitControls } from "three/examples/jsm/controls/OrbitControls.js";
import { GLTFLoader } from "three/examples/jsm/loaders/GLTFLoader.js";
import { DRACOLoader } from "three/examples/jsm/loaders/DRACOLoader.js";
import { RGBELoader } from "three/examples/jsm/loaders/RGBELoader.js";
import { FBXLoader } from "three/examples/jsm/loaders/FBXLoader.js";
import { Menus } from './menus';

export class Contents3D extends Component {
    static displayName = Contents3D.name;

    static T1_1_1F = "T1-1_1F";
    static T1_1_2F = "T1-1_2F";
    static T1_2_1F = "T1-2_1F";
    static T1_2_2F = "T1-2_2F";

    // 0 : Model 이름, 1 : 화면표시, 2 : BoundingBox Model 이름
    static BG_T1 = ["T1", "T1", "T1-0", 57.069950554979215, -272.0369409138239];
    static BG_T2 = ["T2", "T2", "T2-0", 259.51749911683044, -183.2298526860665];
    static BG_T3 = ["T3", "T3", "T3-0", 267.33306396697714, -268.78766338946394];
    static BG_T4 = ["T4", "T4", "T4-0", 56.101643505977478, -183.9927844634657];
    static BG_T5 = ["T5", "T5", "T5-0", 102.8889388113423, -90.72206338349449];
    static BG_T6 = ["T6", "T6", "T6-0", 2.42131208979677, -89.58001188475633];
    static BG_T7 = ["T7", "T7", "T7-0", 487.05955897124886, -50.693945747714494];
    static BG_T8 = ["T8", "T8", "T8-0", -136.7009774818385, 43.012931441842625];
    static BG_T9 = ["T9", "T9", "T9-0", 313.3822158073788, -455.0404417024034];
    static BG_UP = ["UP", "UP", "UP-0", 301.38828949539993, -91.3580747881081];
    static BG_DO_DREAM = ["두드림센터", "두드림센터", "두드림센터-0", 411.4351015183704, -279.48455975724886];
    static BG_SLD = ["SLD", "SLD", "SLD-0", 446.7493320355518, -165.54272832867503];

    constructor(props) {
        super(props);

        this.props = props;
        this.ref3D = React.createRef();

        this.state =
        {
            useBoundingBox: Contents3D.checkContentsBoundingBox(this.props.contents),
            prevInstance: this,
            prevProps: this.props
        };
    }

    initPoiMaterials() {
        this.spriteMaterials = {};
        const _spriteMaterials = this.spriteMaterials;

        const urls = [];
        urls.push('/resource/textures/cup_blue.png');
        urls.push('/resource/textures/cup_white.png');

        urls.forEach((url, index) => {
            const spriteMap = new THREE.TextureLoader().load(url, function (texture) {
                const spriteMaterial = new THREE.SpriteMaterial({ map: spriteMap, color: 0xffffff });
                _spriteMaterials[url] = spriteMaterial;
            });
        });
    }

    static checkContentsBoundingBox(contents) {
        if (contents === Menus.View_All_Outside) {
            return true;
        }

        return false;
    }

    componentDidMount() {
        this.init();
        Contents3D.animate(this.clock, this.renderer, this.scene, this.camera/*, this.stats*/);
        this.loadFile(this.props.contents);

        this.resizeMethod = () => Contents3D.onWindowResize(this.camera, this.renderer);
        window.addEventListener('resize', this.resizeMethod, false);
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.resizeMethod);
        this.detach3D();
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        if (props.contents === state.prevProps.contents) {
            return {
                useBoundingBox: Contents3D.checkContentsBoundingBox(props.contents),
                prevInstance: state.prevInstance,
                prevProps: props
            };
        }

        state.prevInstance.detach3D();
        state.prevInstance.init();
        Contents3D.animate(state.prevInstance.clock, state.prevInstance.renderer, state.prevInstance.scene, state.prevInstance.camera, state.prevInstance.stats);
        state.prevInstance.loadFile(props.contents);

        return {
            useBoundingBox: Contents3D.checkContentsBoundingBox(props.contents),
            prevInstance: state.prevInstance,
            prevProps: props
        };
    }

    cleanMaterial = material => {
        console.log('dispose material!');
        material.dispose();

        // dispose textures
        for (const key of Object.keys(material)) {
            const value = material[key];
            if (value && typeof value === 'object' && 'minFilter' in value) {
                console.log('dispose texture!');
                value.dispose();
            }
        }
    }

    detach3D() {
        this.ref3D.current.removeChild(this.renderer.domElement);

        const meshes = [];
        const materials = [];
        const textures = [];
        const geometries = [];

        this.scene.traverse(obj => {
            if (obj instanceof THREE.Mesh) {
                meshes.push(obj);

                if (obj.geometry instanceof THREE.BufferGeometry) {
                    geometries.push(obj.geometry);
                }

                if (obj.material instanceof THREE.Material) {
                    materials.push(obj.material);

                    if (obj.material.map instanceof THREE.Texture) {
                        textures.push(obj.material.map);
                    }
                }
            }
        });

        this.scene.clear();

        meshes.forEach((obj) => {
            if (obj.parent != null) {
                obj.parent.remove(obj);
            }
            if (obj.dispose) {
                obj.dispose();
            }
        });

        materials.forEach((mat) => {
            if (mat.dispose) {
                mat.dispose();
            }
        });

        textures.forEach((tex) => {
            tex.dispose();
        });

        geometries.forEach((geom) => {
            geom.dispose();
        });

        if (this.scene.background instanceof THREE.Texture) {
            this.scene.background.dispose();
            this.scene.background = null;
        }

        this.renderer.dispose();
        this.renderer = null;
        this.camera = null;
        this.dirLight = null;

        this.scene = null;
        this.clock = null;
    }

    init() {
        this.initPoiMaterials();
        this.boundingBoxModel = null;

        this.boundingBox = null;
        this.boundingBoxModelName = "";
        this.clock = new THREE.Clock();

        //this.camera = new THREE.OrthographicCamera(window.innerWidth / - 2, window.innerWidth / 2, window.innerHeight / 2, window.innerHeight / - 2, 0.1, 5000);
        this.camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 0.1, 5000);
        /*this.camera.quaternion.x = -0.29184374823409764;
        this.camera.quaternion.y = -0.25415844728583714;
        this.camera.quaternion.z = -0.08075295614443928;
        this.camera.quaternion.w = 0.9185367006080156;
        this.camera.rotation.x = -0.4665956919239544;
        this.camera.rotation.y = -0.4674951508389925;
        this.camera.rotation.z = -0.2232067602874086;*/

        this.scene = new THREE.Scene();
        //scene.background = new THREE.Color( 0xa0a0a0 );

        const bgTexture = new THREE.TextureLoader().load('/resource/textures/bg.png');
        this.scene.background = bgTexture;

        //scene.add( new THREE.AmbientLight( 0xffffff, 1.0 ) );

        this.dirLight = new THREE.DirectionalLight(0xffffff, 7.0);
        this.dirLight.position.set(0, 200, 100);
        this.dirLight.castShadow = true;
        this.dirLight.shadow.bias = -0.0008;
        this.dirLight.shadow.mapSize.width = 2048;
        this.dirLight.shadow.mapSize.height = 2048;
        this.dirLight.shadow.camera.updateProjectionMatrix();
        this.scene.add(this.dirLight);
        this.scene.add(this.dirLight.target);

        this.renderer = new THREE.WebGLRenderer({ antialias: true });
        this.renderer.setPixelRatio(window.devicePixelRatio);
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        this.renderer.physicallyCorrectLights = true;
        this.renderer.outputEncoding = THREE.sRGBEncoding;
        this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
        this.renderer.toneMappingExposure = 0.5;
        this.renderer.shadowMap.enabled = true;
        this.renderer.shadowMap.type = THREE.VSMShadowMap;
        this.renderer.shadowMap.autoUpdate = false;
        this.ref3D.current.appendChild(this.renderer.domElement);

        this.scene.add(new THREE.AmbientLight(0x666666, 10));
        /*const pmremGenerator = new THREE.PMREMGenerator(this.renderer);
        pmremGenerator.compileEquirectangularShader();

        const _scene = this.scene;
        const instance = this;

        new RGBELoader()
            .setDataType(THREE.UnsignedByteType)
            .setPath('/resource/gltf/')
            .load('wide_street_01_1k.hdr', function (hdrEquirect) {
                const hdrCubeRenderTarget = pmremGenerator.fromEquirectangular(hdrEquirect);
                hdrEquirect.dispose();
                pmremGenerator.dispose();
                //scene.background = hdrCubeRenderTarget.texture;
                _scene.environment = hdrCubeRenderTarget.texture;
                instance.hdrCubeRenderTarget = hdrCubeRenderTarget;
            });*/

        this.controls = new OrbitControls(this.camera, this.renderer.domElement);
        this.controls.target.set(0, 0, 0);
        this.controls.update();

        // stats(FPS)
        //this.stats = new Stats();
        //this.ref3D.current.appendChild(this.stats.dom);
    }

    addPOI(imgURL, x, z) {
        let spriteMaterial = this.spriteMaterials[imgURL];

        if (!spriteMaterial) {
            const spriteMap = new THREE.TextureLoader().load(imgURL);
            spriteMaterial = new THREE.SpriteMaterial({ map: spriteMap, color: 0xffffff });
        }

        const sprite = new THREE.Sprite(spriteMaterial);

        sprite.material.depthWrite = false;
        sprite.material.depthTest = false;

        sprite.scale.x *= 10;
        sprite.scale.y *= 10;
        sprite.scale.z *= 10;

        sprite.position.x = x;
        sprite.position.z = z;

        this.scene.add(sprite);
        //sprite.material.map.needsUpdate = true
    }

    addBuildingGroupText() {
        /*const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');

        if (context === null) {
            return null;
        }

        const fontSize = 18;
        const fontFace = 'Arial';
        const backgroundColor = { r: 255, g: 255, b: 255, a: 1.0 };
        const borderColor = { r: 0, g: 0, b: 0, a: 1.0 };
        const borderThickness = 4;

        context.font = "Bold " + fontSize + "px " + fontFace;
        context.fillStyle = "rgba(" + backgroundColor.r + "," + backgroundColor.g + "," + backgroundColor.b + "," + backgroundColor.a + ")";
        // border color
        context.strokeStyle = "rgba(" + borderColor.r + "," + borderColor.g + "," + borderColor.b + "," + borderColor.a + ")";
        context.lineWidth = borderThickness;*/

        this.addText(Contents3D.BG_T1[1], Contents3D.BG_T1[3], Contents3D.BG_T1[4]);
        this.addText(Contents3D.BG_T2[1], Contents3D.BG_T2[3], Contents3D.BG_T2[4]);
        this.addText(Contents3D.BG_T3[1], Contents3D.BG_T3[3], Contents3D.BG_T3[4]);
        this.addText(Contents3D.BG_T4[1], Contents3D.BG_T4[3], Contents3D.BG_T4[4]);
        this.addText(Contents3D.BG_T5[1], Contents3D.BG_T5[3], Contents3D.BG_T5[4]);
        this.addText(Contents3D.BG_T6[1], Contents3D.BG_T6[3], Contents3D.BG_T6[4]);
        this.addText(Contents3D.BG_T7[1], Contents3D.BG_T7[3], Contents3D.BG_T7[4]);
        this.addText(Contents3D.BG_T8[1], Contents3D.BG_T8[3], Contents3D.BG_T8[4]);
        this.addText(Contents3D.BG_T9[1], Contents3D.BG_T9[3], Contents3D.BG_T9[4]);
        this.addText(Contents3D.BG_UP[1], Contents3D.BG_UP[3], Contents3D.BG_UP[4]);
        this.addText(Contents3D.BG_DO_DREAM[1], Contents3D.BG_DO_DREAM[3], Contents3D.BG_DO_DREAM[4]);
        this.addText(Contents3D.BG_SLD[1], Contents3D.BG_SLD[3], Contents3D.BG_SLD[4]);
    }

    addText(text, x, z) {
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');

        if (context === null) {
            return null;
        }

        const fontSize = 36;
        const fontFace = 'Arial';
        const backgroundColor = { r: 255, g: 255, b: 255, a: 1.0 };
        const borderColor = { r: 0, g: 0, b: 0, a: 1.0 };
        const borderThickness = 4;

        context.font = "Bold " + fontSize + "px " + fontFace;
        context.fillStyle = "rgba(" + backgroundColor.r + "," + backgroundColor.g + "," + backgroundColor.b + "," + backgroundColor.a + ")";
        // border color
        context.strokeStyle = "rgba(" + borderColor.r + "," + borderColor.g + "," + borderColor.b + "," + borderColor.a + ")";
        context.lineWidth = borderThickness;

        const metrics = context.measureText(text);
        const width = metrics.width + 10;

        Contents3D.roundRect(context, borderThickness / 2, borderThickness / 2, width + borderThickness, fontSize * 1.4 + borderThickness, 6);

        // text color
        context.fillStyle = "rgba(0, 0, 0, 1.0)";

        // metrics.width보다 10만큼 크게 잡았으니 5만큼 띄워서 시작한다.
        context.fillText(text, borderThickness + 5, fontSize + borderThickness);

        // canvas contents will be used for a texture
        const texture = new THREE.Texture(canvas)
        texture.needsUpdate = true;

        //const spriteAlignment = THREE.SpriteAlignment.topLeft;

        const spriteMaterial = new THREE.SpriteMaterial(
            { map: texture, useScreenCoordinates: false/*, alignment: spriteAlignment*/ });
        const sprite = new THREE.Sprite(spriteMaterial);
        sprite.scale.set(100, 50, 1.0);

        sprite.material.depthWrite = false;
        sprite.material.depthTest = false;
        sprite.position.x = x;
        sprite.position.z = z;
        /*sprite.scale.x *= 2;
        sprite.scale.y *= 2;
        sprite.scale.z *= 2;*/

        this.scene.add(sprite);
        return sprite;
    }

    static roundRect(context, x, y, w, h, r) {
        context.beginPath();
        context.moveTo(x + r, y);
        context.lineTo(x + w - r, y);
        context.quadraticCurveTo(x + w, y, x + w, y + r);
        context.lineTo(x + w, y + h - r);
        context.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
        context.lineTo(x + r, y + h);
        context.quadraticCurveTo(x, y + h, x, y + h - r);
        context.lineTo(x, y + r);
        context.quadraticCurveTo(x, y, x + r, y);
        context.closePath();
        context.fill();
        context.stroke();
    }

    // stats : FPS 표시
    static animate(clock, renderer, scene, camera/*, stats*/) {
        requestAnimationFrame(() => {
            Contents3D.animate(clock, renderer, scene, camera/*, stats*/);
        });

        const vLookAt = new THREE.Vector3(camera.matrix[8], camera.matrix[9], camera.matrix[10]);

        const delta = clock.getDelta();
        renderer.render(scene, camera);
        //stats.update();
    }

    loadFile(contents) {
        const fileName = '/resource/gltf/' + contents;
        //const fileName = '/resource/gltf/04_soubrain_1-1-all.glb';
        const worldBox = new THREE.Box3();

        let loader = null;

        if (fileName.endsWith('.fbx')) {
            loader = new FBXLoader();
        } else if (fileName.endsWith('.glb') || fileName.endsWith('.gltf')) {
            loader = new GLTFLoader();
            // Optional: Provide a DRACOLoader instance to decode compressed mesh data
            const dracoLoader = new DRACOLoader();
            dracoLoader.setDecoderPath('/three/examples/js/libs/draco/');
            loader.setDRACOLoader(dracoLoader);
        }

        const worldPos = new THREE.Vector3();
        const worldScale = new THREE.Vector3();
        const worldQuat = new THREE.Quaternion();

        const scene = this.scene;
        const dirLight = this.dirLight;
        const renderer = this.renderer;
        const camera = this.camera;
        const controls = this.controls;
        const _this = this;

        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                child.getWorldPosition(worldPos);
                child.getWorldScale(worldScale);
                child.getWorldQuaternion(worldQuat);

                if (child instanceof THREE.Mesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                    worldBox.expandByObject(child);
                    //console.log(child.material);
                }
            });

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;
            scene.add(modelNode);
            modelNode.updateMatrixWorld(true);

            const boxSize = new THREE.Vector3();
            worldBox.getSize(boxSize);

            const sceneMaxLen = boxSize.length();
            const sceneHalfMaxLen = sceneMaxLen * 0.5;

            worldBox.getCenter(dirLight.target.position);
            dirLight.position.copy(dirLight.target.position);

            const lightPos = new THREE.Vector3(sceneHalfMaxLen, sceneMaxLen, sceneHalfMaxLen);
            dirLight.position.add(lightPos);

            const lightDistance = lightPos.length();

            dirLight.shadow.camera.near = lightDistance - sceneHalfMaxLen;
            dirLight.shadow.camera.far = lightDistance + sceneHalfMaxLen;
            dirLight.shadow.camera.right = sceneHalfMaxLen;
            dirLight.shadow.camera.left = -sceneHalfMaxLen;
            dirLight.shadow.camera.top = sceneHalfMaxLen;
            dirLight.shadow.camera.bottom = -sceneHalfMaxLen;
            dirLight.shadow.camera.updateProjectionMatrix();
            renderer.shadowMap.needsUpdate = true;

            Contents3D.setCamera(camera, controls);

            if (modelNode.name === Menus.View_All_Outside) {
                _this.addBuildingGroupText();
                Contents3D.hideBoundingBoxes(modelNode);
            }
            /*Contents3D.addPOI(scene, '/resource/textures/cup_blue.png', 0, 0);
            Contents3D.addPOI(scene, '/resource/textures/cup_white.png', 20, -30);*/
        });
    }

    static hideBoundingBoxes(obj) {
        let childCount = obj.children.length;

        if (childCount === 1) {
            obj = obj.children[0];
            childCount = obj.children.length;
        }

        // BoundingBox 감추기
        for (let i = 0; i < childCount; i++) {
            const child = obj.children[i];

            if (child.name === Contents3D.BG_T1[2] ||
                child.name === Contents3D.BG_T2[2] ||
                child.name === Contents3D.BG_T3[2] ||
                child.name === Contents3D.BG_T4[2] ||
                child.name === Contents3D.BG_T5[2] ||
                child.name === Contents3D.BG_T6[2] ||
                child.name === Contents3D.BG_T7[2] ||
                child.name === Contents3D.BG_T8[2] ||
                child.name === Contents3D.BG_T9[2] ||
                child.name === Contents3D.BG_UP[2] ||
                child.name === Contents3D.BG_DO_DREAM[2] ||
                child.name === Contents3D.BG_SLD[2]) {
                child.visible = false;
            }
        }
    }

    static setCamera(camera, controls) {
        /*camera.position.set(363.82387194045134, 473.75643926239616, -271.2496985854127);
        camera.quaternion.set(-0.29184374823409764, -0.25415844728583714, -0.08075295614443928, 0.9185367006080156);
        camera.rotation.set(-0.4665956919239544, -0.4674951508389925, -0.2232067602874086);*/
        camera.position.set(372.68334956556555, 611.7531612927072, 421.9390416876601);
        camera.quaternion.set(-0.44010966979818145, -0.08370424553965593, -0.04124934250748849, 0.8930820620589536);
        camera.rotation.set(-0.9242082014112738, -0.11344422502060696, -0.1488586584352319);
        controls.target.set(538.9034870010015, -552.6779707308191, -457.01395028700694);
    }

    changeFile(contents) {
        const fileName = '/resource/gltf/' + contents;
        const worldBox = new THREE.Box3();
        let loader = null;

        if (fileName.endsWith('.fbx')) {
            loader = new FBXLoader();
        } else if (fileName.endsWith('.glb') || fileName.endsWith('.gltf')) {
            loader = new GLTFLoader();
            // Optional: Provide a DRACOLoader instance to decode compressed mesh data
            const dracoLoader = new DRACOLoader();
            dracoLoader.setDecoderPath('/three/examples/js/libs/draco/');
            loader.setDRACOLoader(dracoLoader);
        }

        const worldPos = new THREE.Vector3();
        const worldScale = new THREE.Vector3();
        const worldQuat = new THREE.Quaternion();

        const scene = this.scene;
        
        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                child.getWorldPosition(worldPos);
                child.getWorldScale(worldScale);
                child.getWorldQuaternion(worldQuat);

                if (child instanceof THREE.Mesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                    worldBox.expandByObject(child);
                    console.log(child.material);
                }
            });

            const childCount = scene.children.length;

            for (let i = childCount - 1; i >= 0; i--) {
                const childModel = scene.children[i];

                if (childModel.name.length > 0) {
                    childModel.clear();
                    scene.remove(childModel);
                }
            }

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;
            scene.add(modelNode);
            modelNode.updateMatrixWorld(true);
        });
    }

    static onWindowResize(camera, renderer) {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    onMouseMove = (event) => {
        if (!this.state.useBoundingBox) {
            return;
        }

        this.removeBoundingBox();

        const x = event.nativeEvent.offsetX;
        const y = event.nativeEvent.offsetY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, this.camera);

        const intersects = raycaster.intersectObjects(this.scene.children, true);
        const intersectCount = intersects.length;

        if (intersectCount > 0) {
            const nearestIntersect = this.getNearest(intersects, intersectCount);

            if (nearestIntersect) {
                const model = this.getIntersectModel(nearestIntersect.object);

                if (model !== null) {
                    // BoundingBox Model 사용 버전
                    model.visible = true;
                    this.boundingBoxModel = model;
                    // BoundingBox 사용 버전
                    /*const boundingBox = this.getBoundingBox(model);
                    //const boundingBox = new THREE.BoxHelper(model, 0xff0000);
                    this.scene.add(boundingBox);
                    this.boundingBox = boundingBox;
                    this.boundingBoxModelName = model.name;*/
                }
            }
        }
    }

    onClick = (event) => {
        if (this.props.poi === null) {
            if (!this.state.useBoundingBox || this.boundingBoxModelName.length === 0) {
                /*console.log(`camera.position.set(${this.camera.position.x}, ${this.camera.position.y}, ${this.camera.position.z});`);
                console.log(`camera.quaternion.set(${this.camera.quaternion.x}, ${this.camera.quaternion.y}, ${this.camera.quaternion.z}, ${this.camera.quaternion.w});`);
                console.log(`camera.rotation.set(${this.camera.rotation.x}, ${this.camera.rotation.y}, ${this.camera.rotation.z});`);
                console.log(`controls.target.set(${this.controls.target.x}, ${this.controls.target.y}, ${this.controls.target.z});`);*/
                return;
            }

            if (this.boundingBoxModelName === Contents3D.T1_1_1F ||
                this.boundingBoxModelName === Contents3D.T1_1_2F) {
                this.props.onSelectContents(Menus.View_1_1_1F);
            }
            else if (this.boundingBoxModelName === Contents3D.T1_2_1F ||
                this.boundingBoxModelName === Contents3D.T1_2_2F) {
                this.props.onSelectContents(Menus.View_1_2_1F);
            }
        }
        else {
            const x = event.nativeEvent.offsetX;
            const y = event.nativeEvent.offsetY;
            const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

            const raycaster = new THREE.Raycaster();
            raycaster.setFromCamera(mouse, this.camera);

            const intersects = raycaster.intersectObjects(this.scene.children, true);
            
            if (intersects.length > 0) {
                const pos = intersects[0].point;
                this.addPOI('/resource/textures/' + this.props.poi, pos.x, pos.z);
            }
        }
    }

    /*onClick = (event) => {
        this.removeBoundingBox();

        const x = event.nativeEvent.offsetX;
        const y = event.nativeEvent.offsetY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, this.camera);

        const intersects = raycaster.intersectObjects(this.scene.children, true);
        const intersectCount = intersects.length;

        if (intersectCount > 0) {
            const nearestIntersect = this.getNearest(intersects, intersectCount);

            if (nearestIntersect) {
                const model = this.getIntersectModel(nearestIntersect.object);

                if (model !== null) {
                    const boundingBox = this.getBoundingBox(model);
                    this.scene.add(boundingBox);
                    this.boundingBox = boundingBox;
                }
            }
        }
    }*/

    removeBoundingBox() {
        if (this.boundingBox) {
            this.scene.remove(this.boundingBox);
            this.boundingBox.geometry.dispose();
            this.boundingBox.material.dispose();
            this.boundingBox = null;
            this.boundingBoxModelName = "";
        }

        if (this.boundingBoxModel) {
            this.boundingBoxModel.visible = false;
            this.boundingBoxModel = null;
        }
    }

    getBoundingBox(model) {
        /*let otherModel = null;

        if (model.name === Contents3D.T1_1_1F) {
            otherModel = this.getChildModel(model.parent, Contents3D.T1_1_2F);
        }
        else if (model.name === Contents3D.T1_1_2F) {
            otherModel = this.getChildModel(model.parent, Contents3D.T1_1_1F);
        }
        else if (model.name === Contents3D.T1_2_1F) {
            otherModel = this.getChildModel(model.parent, Contents3D.T1_2_2F);
        }
        else if (model.name === Contents3D.T1_2_2F) {
            otherModel = this.getChildModel(model.parent, Contents3D.T1_2_1F);
        }*/

        const boundingBox = new THREE.BoxHelper(model, 0xff0000);

        /*if (otherModel === null) {
            return boundingBox;
        }

        const boundingBox2 = new THREE.BoxHelper(otherModel, 0xff0000);
        this.unionBoundingBox(boundingBox, boundingBox2);

        boundingBox2.geometry.dispose();
        boundingBox2.material.dispose();*/

        return boundingBox;
    }

    unionBoundingBox(box1, box2) {
        const maxX1 = box1.geometry.attributes.position.array[0];
        const maxX2 = box2.geometry.attributes.position.array[0];
        const maxY1 = box1.geometry.attributes.position.array[1];
        const maxY2 = box2.geometry.attributes.position.array[1];
        const maxZ1 = box1.geometry.attributes.position.array[2];
        const maxZ2 = box2.geometry.attributes.position.array[2];

        const minX1 = box1.geometry.attributes.position.array[18];
        const minX2 = box2.geometry.attributes.position.array[18];
        const minY1 = box1.geometry.attributes.position.array[19];
        const minY2 = box2.geometry.attributes.position.array[19];
        const minZ1 = box1.geometry.attributes.position.array[20];
        const minZ2 = box2.geometry.attributes.position.array[20];

        const maxX = maxX1 > maxX2 ? maxX1 : maxX2;
        const maxY = maxY1 > maxY2 ? maxY1 : maxY2;
        const maxZ = maxZ1 > maxZ2 ? maxZ1 : maxZ2;

        const minX = minX1 < minX2 ? minX1 : minX2;
        const minY = minY1 < minY2 ? minY1 : minY2;
        const minZ = minZ1 < minZ2 ? minZ1 : minZ2;

        box1.geometry.attributes.position.array[0] = maxX;
        box1.geometry.attributes.position.array[1] = maxY;
        box1.geometry.attributes.position.array[2] = maxZ;
        box1.geometry.attributes.position.array[3] = minX;
        box1.geometry.attributes.position.array[4] = maxY;
        box1.geometry.attributes.position.array[5] = maxZ;
        box1.geometry.attributes.position.array[6] = minX;
        box1.geometry.attributes.position.array[7] = minY;
        box1.geometry.attributes.position.array[8] = maxZ;
        box1.geometry.attributes.position.array[9] = maxX;
        box1.geometry.attributes.position.array[10] = minY;
        box1.geometry.attributes.position.array[11] = maxZ;
        box1.geometry.attributes.position.array[12] = maxX;
        box1.geometry.attributes.position.array[13] = maxY;
        box1.geometry.attributes.position.array[14] = minZ;
        box1.geometry.attributes.position.array[15] = minX;
        box1.geometry.attributes.position.array[16] = maxY;
        box1.geometry.attributes.position.array[17] = minZ;
        box1.geometry.attributes.position.array[18] = minX;
        box1.geometry.attributes.position.array[19] = minY;
        box1.geometry.attributes.position.array[20] = minZ;
        box1.geometry.attributes.position.array[21] = maxX;
        box1.geometry.attributes.position.array[22] = minY;
        box1.geometry.attributes.position.array[23] = minZ;
    }

    getChildModel(parent, childName) {
        const childCount = parent.children.length;

        for (let i = 0; i < childCount; i++) {
            const child = parent.children[i];

            if (child.name === childName) {
                return child;
            }
        }

        return null;
    }

    getIntersectModel(obj) {
        if (obj.name === Contents3D.BG_T1[2] ||
            obj.name === Contents3D.BG_T2[2] ||
            obj.name === Contents3D.BG_T3[2] ||
            obj.name === Contents3D.BG_T4[2] ||
            obj.name === Contents3D.BG_T5[2] ||
            obj.name === Contents3D.BG_T6[2] ||
            obj.name === Contents3D.BG_T7[2] ||
            obj.name === Contents3D.BG_T8[2] ||
            obj.name === Contents3D.BG_T9[2] ||
            obj.name === Contents3D.BG_UP[2] ||
            obj.name === Contents3D.BG_DO_DREAM[2] ||
            obj.name === Contents3D.BG_SLD[2]) {
            return obj;
        }

        if (obj.parent === null) {
            return null;
        }

        return this.getIntersectModel(obj.parent);
    }

    getNearest(objects, objectCount) {
        let min = objects[0].distance;
        let obj = objects[0];

        for (let i = 1; i < objectCount; i++) {
            const distance = objects[i].distance;

            if (distance < min) {
                min = distance;
                obj = objects[i];
            }
        }

        return obj;
    }

    render() {
        return (
            <div ref={this.ref3D} className={styles.contentsArea} onClick={this.onClick} onMouseMove={this.onMouseMove}>
            </div>
        );
    }
}

export default Contents3D;