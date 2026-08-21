import React, { Component } from 'react';
import * as THREE from "three/build/three.module.js";
import { Vector3 } from 'three';
import { OrbitControls } from "three/examples/jsm/controls/OrbitControls.js";
import { GLTFLoader } from "three/examples/jsm/loaders/GLTFLoader.js";
import { DRACOLoader } from "three/examples/jsm/loaders/DRACOLoader.js";
import { FBXLoader } from "three/examples/jsm/loaders/FBXLoader.js";
import ProgressBar from './progressBar';
import styles from '../css/_3d.module.css';
import { SpaceDataManager } from '../services/spaceDataManager';
import { AnimationModel } from './components/animationModel';
import Toolbar from './toolbar';
import { SpaceMenus } from '../spaceMenus';
import { SpatialManager } from './spatialManager';
import Geometry from '../../utility/Geometry';
import { SpaceBody } from '../spaceBody';
//import { ModelDataManager } from '../services/modelDataManager';
import { CameraManager } from './cameraManager';
//import $ from 'jquery';
import { POIManager } from './poiManager';
import { POIEdit } from '../basicInfo/poiEdit';

export class Contents3D extends Component {
    static Mode_Outdoor_All = 0;
    static Mode_Outdoor_Part = 1;
    static Mode_Indoor = 2;

    static ExitArrowGroupTag = "arrow_Group";
    static ExitArrowBeginTag = "arrow_Y";
    static ExitArrowEndTag = "arrow_R";

    static FacilityHeadTag = "equipment-";

    static Edit_Mode_None = 0;
    static Edit_Mode_MovePOI = 1;
    static Edit_Mode_FakeWall = 2;
    static Edit_Mode_Text = 3;
    static Edit_Mode_CCTVGroup = 4;

    constructor(props) {
        super(props);

        this.props = props;
        this.ref3D = React.createRef();

        this.state =
        {
            loading: false,
            prevInstance: this,
            prevProps: this.props,
            progressActive: true,
            progressValue: 0,
            editableInput: false,
            visibleSensorTypes: Contents3D.makeVisibleSensorTypes()
        };

        this.indoorModelCount = 0;
        this.indoorModelCountTemp = 0;
        this.clock = new THREE.Clock();
        this.boundingBoxModel = null;
        this.useBoundingBox = true;
        this.renderer = null;
        this.scene = null;
        this.camera = null;
        this.dirLight = null;
        this.controls = null;
        this.currentModel = null;
        this.currentIndoorModel = null;
        this.outdoorModels = [];
        this.outdoorFacilities = {};
        this.internalModels = {};
        
        this.prevIndoorModel = null;
        this.movingCamera = null;

        this.blinkDatas = [];
        this.movingDatas = [];

        this.outdoorModelTotalCount = 0;
        this.outdoorModelTotalCountTemp = 0;
        this.outdoorModelCount = -1;
        this.completeOutdoorModelCount = -1;

        this.pickPOI = null;
        this.perspectiveCamera = null;
        this.orthoGraphicCamera = null;
        this.perspectiveControlOrigin = new Vector3(0, 0, 0);

        this.loadingSiteIDs = [];                                                                                                                            
        this.initialized1 = false;
        this.initialized2 = false;

        this.facilityMaps = {};

        // 모델 파일별 Animation
        // Key : ModelFile Name
        // Value : AnimationModel
        this.modelAnimations = {};
        this.currentAnimationModels = [];

        this.setDirectionalLightPower();

        //this.poiManager = new POIManager(this);
    }

    componentDidMount() {
        window.progressbar = this;
        const _3dOptions = this.props._3dOptions;

        this.poiManager = this.props.poiManager;
        this.poiManager.Contents3D = this;

        if (_3dOptions.outdoorModel && this.initialized1 === false && this.initialized2 === false) {
            this.init();
            Contents3D.animate(this);

            this.loadingSiteIDs.push(this.props.currentSiteID);

            /*if (this.props.multiSite) {
                this.setOutdoorModelCount();
            }*/

            const modelFiles = this.getOutdoorModelFiles(_3dOptions);
            this.loadOutdoorModelFiles(modelFiles, _3dOptions, true);
            
            this.resizeMethod = () => Contents3D.onWindowResize(this.camera, this.renderer);
            window.addEventListener('resize', this.resizeMethod, false);
            window.addEventListener('keydown', this.onKeyDown, false);
        }


        /* $(document).ready(function () {
            var button = $('.' + styles.moveMe);
            var bar = $('.' + styles.progressBar);
            var barWidth = bar.outerWidth();
            var clickPosition;
            var percentage = 0;
            var buttonPosition;
            $(window).resize(function () {
                barWidth = bar.outerWidth();
                setButton();
            });
            var setButton = function () {
                buttonPosition = percentage * barWidth - 10;
                button.css("width", buttonPosition + 'px');
            };
            $('.' + styles.progressBar).click(function (e) {
                clickPosition = e.pageX - $(this).offset().left;
                percentage = clickPosition / barWidth;
                setButton();
                $('.' + styles.percentage).text(Math.round(percentage * 100) + "%");
            });
        });


        $(document).ready(function () {
            var button = $('.' + styles.moveMe2);
            var bar = $('.' + styles.progressBar2);
            var barWidth = bar.outerWidth();
            var clickPosition;
            var percentage = 0;
            var buttonPosition;
            $(window).resize(function () {
                barWidth = bar.outerWidth();
                setButton();
            });
            var setButton = function () {
                buttonPosition = percentage * barWidth - 10;
                button.css("width", buttonPosition + 'px');
            };
            $('.' + styles.progressBar2).click(function (e) {
                clickPosition = e.pageX - $(this).offset().left;
                percentage = clickPosition / barWidth;
                setButton();
                $('.' + styles.percentage2).text(Math.round(percentage * 100) + "%");
            });
        });


        $(document).ready(function () {
            $('.' + styles.illuminanceSet).hover(function () {
                $('.' + styles.lightBox).show();
            }, function () {
                $('.' + styles.lightBox).hide();
            })
        }); */
    }

    componentWillUnmount() {
        this.initialized1 = false;
        window.removeEventListener('resize', this.resizeMethod);
        window.removeEventListener('keydown', this.onKeyDown);
        this.detach3D();
        this.initialized2 = false;
        this.timelog("componentIsUnmounted");
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        state.prevInstance.poiManager = props.poiManager;
        state.prevInstance.poiManager.Contents3D = state.prevInstance;

        state.prevInstance.checkCameraMode(props);
        Contents3D.processMenu(props, state);

        return {
            loading: state.loading,
            prevInstance: state.prevInstance,
            prevProps: props,
            progressActive: state.progressActive,
            progressValue: state.progressValue,
            editableInput: state.editableInput
        };
    }

    static processMenu(props, state) {
        if (props.command) {
            if (props.command.menu === SpaceMenus.Menu_Show_Outdoor) {
                const model = props.command.menuParameter;
                state.prevInstance.setMovingCamera(model.camera, Contents3D.Mode_Outdoor_All);
            }
            else if (props.command.menu === SpaceMenus.Menu_MoveTo_Floor) {
                const [buildingID, floorIndex] = props.command.menuParameter;

                if (buildingID !== undefined && buildingID !== null &&
                    floorIndex !== undefined && floorIndex !== null && isNaN(floorIndex) === false) {
                    state.prevInstance.moveToFloor(buildingID, floorIndex);
                }
                else {
                    props.initOutdoorViewport();
                }
            }
            else if (props.command.menu === SpaceMenus.Menu_MoveTo_Building) {
                const buildingName = props.command.menuParameter;
                state.prevInstance.moveToBuilding(buildingName);
            }
            else if (props.command.menu === SpaceMenus.Menu_MoveTo_BuildingGroup) {
                const buildingGroupName = props.command.menuParameter;

                if (buildingGroupName) {
                    if (Array.isArray(buildingGroupName) === false) {
                        state.prevInstance.moveToBuildingGroup(buildingGroupName);
                    }
                    else if (buildingGroupName.length > 0) {
                        state.prevInstance.moveToBuildingGroup(buildingGroupName[0]);
                    }
                }
                else {
                    props.initOutdoorViewport();
                }
            }

            props.command.menu = SpaceMenus.Menu_None;
            props.command.menuParameter = null;
        }
    }

    static makeVisibleSensorTypes() {
        const visibleSensorTypes = {};

        visibleSensorTypes[SpaceDataManager.FireSensorType] = true;
        visibleSensorTypes[SpaceDataManager.PSMSensorType] = true;
        visibleSensorTypes[SpaceDataManager.EtcSensorType] = true;
        visibleSensorTypes[SpaceDataManager.CCTVType] = true;

        return visibleSensorTypes;
    }

    checkCameraMode(props) {
        if (props.editMode && this.camera === this.perspectiveCamera) {
            this.changeCamera(true);
        }
        else if (props.editMode === false && this.camera === this.orthoGraphicCamera) {
            this.changeCamera(false);
        }
    }

    setDirectionalLightPower() {
        this.directionalLightPower = 6;
    }

    getOutdoorModelFiles(_3dOptions) {
        const modelFiles = [];
        modelFiles.push([_3dOptions.outdoorModel.file, SpaceBody.Type_Site, null]);

        for (const buildingGroupName in _3dOptions.indoorModels) {
            const buildingGroup = _3dOptions.indoorModels[buildingGroupName];

            if (buildingGroup && buildingGroup.file) {
                modelFiles.push([buildingGroup.file, SpaceBody.Type_BuildingGroup, buildingGroup.buildingGroupID]);
            }
        }

        return modelFiles;
    }

    detach3D() {
        if (!this.ref3D.current || !this.renderer) {
            return;
        }

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
            if (obj.parent !== null) {
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

        this.boundingBoxModel = null;
        this.renderer = null;
        this.scene = null;
        this.camera = null;
        this.dirLight = null;
        this.controls = null;
        this.currentModel = null;
        this.internalModels = {};
    }

    init() {
        this.initialized1 = true;
        this.internalModels = {};
        
        const outdoorModel = this.props._3dOptions.outdoorModel;

        this.orthoGraphicCamera = new THREE.OrthographicCamera(window.innerWidth / - 2, window.innerWidth / 2, window.innerHeight / 2, window.innerHeight / - 2, 0.1, 5000);
        this.perspectiveCamera = new THREE.PerspectiveCamera(outdoorModel.camera.fov, window.innerWidth / window.innerHeight, outdoorModel.camera.near, outdoorModel.camera.far);
        this.camera = this.props.editMode ? this.orthoGraphicCamera : this.perspectiveCamera;

        this.scene = new THREE.Scene();
        this.poiManager.Scene = this.scene;

        const bgTexture = new THREE.TextureLoader().load(this.props._3dOptions.textureBaseURL + '/' + this.props._3dOptions.backgroundImage);
        this.scene.background = bgTexture;

        const hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444, 0.1);
        hemiLight.position.set(0, 20, 0);
        this.scene.add(hemiLight);

        this.dirLight = new THREE.DirectionalLight(0xffffff/*, this.directionalLightPower*/);
        this.dirLight.position.set(-3, 10, -10);
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
        this.renderer.outputEncoding = THREE.sRGBEncoding;
        this.renderer.shadowMap.enabled = true;
        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
        this.ref3D.current.appendChild(this.renderer.domElement);

        this.controls = new OrbitControls(this.camera, this.renderer.domElement);
        this.controls.target.set(0, 0, 0);
        // 최대 회전각
        this.controls.maxPolarAngle = Math.PI / 3;
        this.controls.update();
        this.initialized2 = true;
    }

    static animate(_this) {
        requestAnimationFrame(() => {
            Contents3D.animate(_this);
        });

        const delta = _this.clock.getDelta();

        if (_this.movingCamera) {
            _this.moveCamera(delta);
        }

        if (_this.renderer && _this.scene && _this.camera && _this.initialized1 && _this.initialized2) {
            _this.renderer.render(_this.scene, _this.camera);
        }
    }

    static onWindowResize(camera, renderer) {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    loadOutdoorModelFiles(modelFiles, _3dOptions, visible) {
        this.timelog("Begin Loading");
        const fileCount = modelFiles.length;

        this.outdoorModelCount = fileCount;
        this.completeOutdoorModelCount = 0;

        if (fileCount > 0) {
            this.loadRootModel(modelFiles[0][0], 1, modelFiles, Contents3D.Mode_Outdoor_All, visible, _3dOptions);
        }

        this.loadComponentModels();
    }

    loadComponentModels() {
    }

    loadRootModel(contents, nextIndex, files, mode, visible, _3dOptions) {
        this.setState({ loading: true });

        const fileName = _3dOptions.modelBaseURL + "/" + contents;
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

        const _this = this;
        //const cameraOptions = _3dOptions.outdoorModel.camera;

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
                }
            });

            const isIndoor = _this.isIndoor();

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;

            if (isIndoor) {
                modelNode.visible = false;
            }
            else {
                modelNode.visible = visible;
            }

            const scene = _this.scene;

            if (scene) {
                scene.add(modelNode);
                modelNode.updateMatrixWorld(true);

                // AnimationModel이 있는지 확인한다.
                _this.loadAnimationModels(object, modelNode);

                _this.removeBoundingBoxShadow(modelNode);

                const boxSize = new THREE.Vector3();
                worldBox.getSize(boxSize);

                const sceneMaxLen = boxSize.length();
                const sceneHalfMaxLen = sceneMaxLen * 0.5;

                const dirLight = _this.dirLight;
                const cameraPerspective = _this.perspectiveCamera;
                const cameraOrtho = _this.orthoGraphicCamera;
                const renderer = _this.renderer;
                const controls = _this.controls;

                if (dirLight && cameraPerspective && cameraOrtho && renderer) {
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

                    const [min, max] = CameraManager.getBoundingBoxMinMax(modelNode);

                    if (visible) {
                        CameraManager.setControlCamera(cameraPerspective, controls, min, max, _3dOptions.outdoorModel.camera, false);
                        CameraManager.setControlCameraOrtho(cameraOrtho, min, max);
                    }

                    if (isIndoor === false && visible) {
                        _this.currentModel = modelNode;
                    }

                    _this.onCompleteOutdoorModelLoading(modelNode, _3dOptions);

                    if (mode === Contents3D.Mode_Outdoor_All || mode === Contents3D.Mode_Outdoor_Part) {
                        _this.useBoundingBox = true;
                        Contents3D.hideBoundingBoxes(modelNode, _3dOptions.buildingGroups, _3dOptions.buildings);
                    }

                    _this.setState({ loading: false });

                    if (nextIndex !== null && nextIndex !== undefined && files) {
                        if (nextIndex < files.length) {
                            for (let i = nextIndex; i < files.length; i++) {
                                _this.loadFile(files[i], visible, null, Contents3D.Mode_Outdoor_Part, _3dOptions);
                            }
                        }
                    }

                    if (isIndoor === false) {
                        _this.poiManager.addOutdoorSensors(_3dOptions.outdoorZones, _3dOptions.zones, _this.state.visibleSensorTypes);
                    }
                }
            }
        });
    }

    loadFile(contents, visible, cameraOptions, mode, _3dOptions, postMethod = null, postMethodParam = null) {
        if (visible) {
            this.setState({ loading: true });
        }

        const fileName = _3dOptions.modelBaseURL + "/" + contents[0];
        const contentsType = contents[1];
        const contentsID = contents[2];

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

        const _this = this;
        const cameraPerspective = _this.perspectiveCamera;
        //const cameraOrtho = _this.orthoGraphicCamera;
        const controls = _this.controls;

        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                if (child instanceof THREE.Mesh) {
                    child.castShadow = mode !== Contents3D.Mode_Indoor;
                    child.receiveShadow = mode !== Contents3D.Mode_Indoor;
                }
            });

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents[0];

            if (_this.isIndoor() && mode !== Contents3D.Mode_Indoor) {
                // 실내모드일 경우 외부모델 파일을 로딩하면 무조건 안보이도록 한다.
                modelNode.visible = false;
            }
            else {
                modelNode.visible = visible;
            }

            const scene = _this.scene;

            if (scene) {
                scene.add(modelNode);
                modelNode.updateMatrixWorld(true);

                // AnimationModel이 있는지 확인한다.
                _this.loadAnimationModels(object, modelNode);

                if (mode === Contents3D.Mode_Outdoor_All || mode === contents.Mode_Outdoor_Part) {
                    Contents3D.hideBoundingBoxes(modelNode, _3dOptions.buildingGroups, _3dOptions.buildings);
                }

                if (!cameraOptions || CameraManager.isEmptyCameraOptions(cameraOptions)) {
                    const [min, max] = CameraManager.getBoundingBoxMinMax(modelNode);

                    cameraOptions = CameraManager.getIndoorModelCamera(contentsType, contentsID, _3dOptions, false);
                    const orthoCameraOptions = CameraManager.setControlCamera(cameraPerspective, controls, min, max, cameraOptions, true);

                    if (orthoCameraOptions) {
                        CameraManager.setIndoorModelCamera(orthoCameraOptions, contentsType, contentsID, _3dOptions, true);
                    }
                    //Contents3D.setControlCameraOrtho(cameraOrtho, min, max);
                }

                if (cameraOptions) {
                    cameraOptions.fov = _this.camera.fov;
                    cameraOptions.far = _this.camera.far;
                    cameraOptions.near = _this.camera.near;
                }

                if (mode === Contents3D.Mode_Outdoor_Part/*visible*/) {
                    _this.onCompleteOutdoorModelLoading(modelNode, _3dOptions);
                }
                else {
                    const exitArrowData = Contents3D.showExit(modelNode, false);
                    _this.internalModels[contents[0]] = [modelNode, cameraOptions, exitArrowData && exitArrowData.length >= 1 ? exitArrowData[0] : null, exitArrowData && exitArrowData.length >= 2 ? exitArrowData[1] : null];

                    const facilityGroup = Contents3D.showFacilities(modelNode, false, _this.facilityMaps);
                    _this.internalModels[contents[0]].push(facilityGroup);
                }

                if (mode === Contents3D.Mode_Indoor && _this.indoorModelCount > 0) {
                    _this.indoorModelCountTemp++;
                    const rate = _this.indoorModelCountTemp / _this.indoorModelCount * 100;

                    if (rate >= 100) {
                        _this.setState({ progressValue: rate, progressActive: false });

                        if (_3dOptions.indoorModelOnMemory) {
                            // 실내 모델을 메모리에 미리 로딩해 놓고 필요할때 꺼내어 쓰는 경우
                            //_this.loadNextSiteModels();
                        }
                    } else {
                        _this.setState({ progressValue: rate });
                    }
                }

                _this.timelog(contents[0]);
                _this.setState({ loading: false });

                if (postMethod) {
                    if (postMethodParam !== null) {
                        postMethod(postMethodParam);
                    }
                    else {
                        postMethod();
                    }
                }
            }
        });
    }

    // 실내공간 로딩
    async loadIndoorModels(_3dOptions) {
        let fileCount = 0;
        for (const buildingGroupName in _3dOptions.indoorModels) {
            const buildingGroup = _3dOptions.indoorModels[buildingGroupName];

            for (const buildingName in buildingGroup) {
                const building = buildingGroup[buildingName];

                if (building && building.floors) {
                    const floorCount = building.floors.length;

                    for (let i = 0; i < floorCount; i++) {
                        const floor = building.floors[i];

                        if (floor.file && floor.camera) {
                            fileCount++;
                        }
                    }
                }
            }
        }

        this.indoorModelCount = fileCount;
        for (const buildingGroupName in _3dOptions.indoorModels) {
            const buildingGroup = _3dOptions.indoorModels[buildingGroupName];

            for (const buildingName in buildingGroup) {
                const building = buildingGroup[buildingName];

                if (building && building.floors) {
                    const floorCount = building.floors.length;

                    for (let i = 0; i < floorCount; i++) {
                        const floor = building.floors[i];

                        if (floor.file && floor.camera) {
                            //this.addEquipZoneText(floor.zoneID, _3dOptions);
                            this.loadFile([floor.file, SpaceBody.Type_Floor, floor.zoneID], false, floor.camera, Contents3D.Mode_Indoor, _3dOptions);
                        }
                    }
                }
            }
        }
    }

    timelog(log) {
        const now = new Date();
        const time = now.getMinutes() + ":" + now.getSeconds();
        console.log(time + " : " + log);
    }

    onCompleteOutdoorModelLoading(modelNode, _3dOptions) {
        this.completeOutdoorModelCount = this.completeOutdoorModelCount + 1;

        if (this.completeOutdoorModelCount >= this.outdoorModelCount) {
            if (_3dOptions.indoorModelOnMemory) {
                this.loadIndoorModels(_3dOptions);
            }
            else {
                // 실내 모델을 필요할 때에만 실시간으로 로딩하는 경우
                //this.loadNextSiteModels();
            }
        }

        let outdoorModels = this.outdoorModels;
        outdoorModels.push(modelNode);

        const animationModel = this.modelAnimations[modelNode.name];

        if (animationModel) {
            // 외부 모델들을 불러오는 도중이다.
            // 하나씩 외부 모델들이 추가된다.
            this.currentAnimationModels.push(animationModel);
        }
    }

    loadAnimationModels(object, modelNode) {
        if (!object?.animations) {
            return;
        }

        if (object.animations.length > 0) {
            const mixer = new THREE.AnimationMixer(modelNode);

            for (let i = 0; i < object.animations.length; i++) {
                mixer.clipAction(object.animations[i]).play();;
            }

            const animationModel = new AnimationModel(mixer, modelNode);
            this.modelAnimations[modelNode.name] = animationModel;
        }
    }

    // BoundingBox 모델의 그림자를 없앤다.
    removeBoundingBoxShadow(modelNode) {
        if (modelNode.name.endsWith(SpaceDataManager.BoundingBoxTag)) {
            modelNode.castShadow = false;
            modelNode.receiveShadow = false;
        }

        const childCount = modelNode.children.length;

        for (let i = 0; i < childCount; i++) {
            const child = modelNode.children[i];
            this.removeBoundingBoxShadow(child);
        }
    }

    static showFacilities(modelNode, visible, facilityMaps) {
        const childCount = modelNode.children.length;

        if (modelNode.name.startsWith(Contents3D.FacilityHeadTag) && modelNode.name.endsWith(SpaceDataManager.BoundingBoxTag)) {
            for (let i = 0; i < childCount; i++) {
                const child = modelNode.children[i];
                facilityMaps[child.name] = child;

                if (visible) {
                    child.visible = visible;
                }
                else {
                    child.visible = false;
                }
            }

            return modelNode;
        }

        for (let i = 0; i < childCount; i++) {
            const child = Contents3D.showFacilities(modelNode.children[i], visible, facilityMaps);

            if (child !== null) {
                return child;
            }
        }

        return null;
    }

    static showExit(modelNode, visible) {
        if (modelNode.name.startsWith(Contents3D.ExitArrowGroupTag)) {
            if (visible) {
                modelNode.visible = visible;
            }
            else {
                modelNode.visible = false;
            }

            return [modelNode, Contents3D.setArrowDatas(modelNode)];
        }

        const childCount = modelNode.children.length;

        for (let i = 0; i < childCount; i++) {
            const child = Contents3D.showExit(modelNode.children[i], visible);

            if (child !== null) {
                return child;
            }
        }

        return null;
    }

    static setArrowDatas(modelNode) {
        const childCount = modelNode.children.length;
        const datas = {};

        const beginLength = Contents3D.ExitArrowBeginTag.length;
        const endLength = Contents3D.ExitArrowEndTag.length;

        for (let i = 0; i < childCount; i++) {
            const child = modelNode.children[i];

            if (child.name.startsWith(Contents3D.ExitArrowBeginTag)) {
                const tagName = child.name.substring(beginLength);
                let data = datas[tagName];

                if (data) {
                    data.begin = child;
                }
                else {
                    data = { begin: child };
                    datas[tagName] = data;
                }
            }
            else if (child.name.startsWith(Contents3D.ExitArrowEndTag)) {
                const tagName = child.name.substring(endLength);
                let data = datas[tagName];

                if (data) {
                    data.end = child;
                }
                else {
                    data = { end: child };
                    datas[tagName] = data;
                }
            }
        }

        const arrowDatas = [];

        for (const key in datas) {
            const data = datas[key];

            if (data.begin && data.end) {
                data.end.visible = false;
                const distance = Geometry.getDistance3(data.begin.position.x, data.begin.position.y, data.begin.position.z, data.end.position.x, data.end.position.y, data.end.position.z);
                arrowDatas.push([data.begin, new Vector3(data.begin.position.x, data.begin.position.y, data.begin.position.z), new Vector3(data.end.position.x, data.end.position.y, data.end.position.z), distance]);
            }
        }

        return arrowDatas;
    }

    static hideBoundingBoxes(obj, buildingGroups, buildings) {
        let childCount = obj.children.length;

        if (childCount === 1) {
            obj = obj.children[0];
            childCount = obj.children.length;
        }

        // BoundingBox 감추기
        for (let i = 0; i < childCount; i++) {
            const child = obj.children[i];

            if (child.name.endsWith(SpaceDataManager.BoundingBoxTag)) {
                child.visible = false;
            }
        }
    }

    isIndoor() {
        if (!this.currentModel) {
            return false;
        }

        if (!this.props._3dOptions || !this.props._3dOptions.outdoorModel) {
            return false;
        }

        if (this.currentModel.name === this.props._3dOptions.outdoorModel.file) {
            return false;
        }

        return true;
    }

    changeCamera(orthoMode) {
        if (orthoMode) {
            if (this.camera === this.orthoGraphicCamera) {
                return;
            }

            this.camera = this.orthoGraphicCamera;
            this.controls.object = this.camera;
            this.controls.enableRotate = false;
        }
        else {
            if (this.camera === this.perspectiveCamera) {
                return;
            }

            this.camera = this.perspectiveCamera;
            this.camera.updateProjectionMatrix();
            this.controls.object = this.camera;
            this.controls.enableRotate = true;
        }

        const currentView = this.props.currentView;

        if (this.isIndoor() && currentView) {
            if (currentView.zoneID !== null && currentView.zoneID !== undefined) {
                const cameraOptions = CameraManager.getIndoorModelCamera(SpaceBody.Type_Floor, currentView.zoneID, this.props._3dOptions, orthoMode);

                if (CameraManager.isEmptyCameraOptions(cameraOptions) === false) {
                    CameraManager.setCamera(this.camera, this.controls, cameraOptions);
                    this.camera.updateProjectionMatrix();
                }
            }
        }
    }

    setMovingCamera(cameraOptions, mode, param, speedUpRatio) {
        this.setState({ loading: true });

        const distancePos = Geometry.getDistance3(this.camera.position.x, this.camera.position.y, this.camera.position.z, cameraOptions.position[0], cameraOptions.position[1], cameraOptions.position[2]);
        const distanceQua = cameraOptions.quaternion === null ? null : Geometry.getDistance4(this.camera.quaternion.x, this.camera.quaternion.y, this.camera.quaternion.z, this.camera.quaternion.w, cameraOptions.quaternion[0], cameraOptions.quaternion[1], cameraOptions.quaternion[2], cameraOptions.quaternion[3]);
        const distanceRot = Geometry.getDistance3(this.camera.rotation.x, this.camera.rotation.y, this.camera.rotation.z, cameraOptions.rotation[0], cameraOptions.rotation[1], cameraOptions.rotation[2]);

        const movingTime = 0.75;
        let speedUp = null;

        if (speedUpRatio !== undefined && speedUpRatio !== null && speedUpRatio.length >= 2) {
            speedUp = {
                begin: movingTime * speedUpRatio[0],
                end: movingTime * speedUpRatio[1]
            }
        }

        this.movingCamera = {
            // 초
            movingTime: movingTime,
            //movingTime: 1.5,
            elapsedTime: 0,
            speedUp: speedUp,
            distancePosition: distancePos,
            distanceQuaternion: distanceQua,
            distanceRotation: distanceRot,
            beginCameraPos: new THREE.Vector3(this.camera.position.x, this.camera.position.y, this.camera.position.z),
            beginCameraQuaternion: new THREE.Quaternion(this.camera.quaternion.x, this.camera.quaternion.y, this.camera.quaternion.z, this.camera.quaternion.w),
            beginCameraRotation: new THREE.Vector3(this.camera.rotation.x, this.camera.rotation.y, this.camera.rotation.z),
            targetCameraOptions: cameraOptions,
            fov: cameraOptions.fov,
            far: cameraOptions.far,
            near: cameraOptions.near,
            mode: mode,
            param: param
        };

        // 실내로 이동할 때에는 이동이 끝난후에 outdoor를 감춘다.
        if (mode !== Contents3D.Mode_Indoor) {
            this.showOutdoor(mode);
        }
    }

    moveCamera(delta) {
        const cameraOptions = CameraManager.makeEmptyCameraOptions(false);
        cameraOptions.targetControl = [...this.movingCamera.targetCameraOptions.targetControl];
        /*const cameraOptions = {
            position: [],
            quaternion: [],
            rotation: [],
            targetControl: [...this.movingCamera.targetCameraOptions.targetControl]
        }*/

        if (this.movingCamera) {
            if (this.movingCamera.elapsedTime === 0) {
                this.timelog("begin camera move");
            }

            if (this.movingCamera.speedUp !== null && this.movingCamera.speedUp !== undefined && this.movingCamera.elapsedTime >= this.movingCamera.speedUp.begin && this.movingCamera.elapsedTime <= this.movingCamera.speedUp.end) {
                // speedUp 구간에서는 속도를 2배로 올린다.
                this.movingCamera.elapsedTime += delta * 2;
            }
            else {
                this.movingCamera.elapsedTime += delta;
            }

            if (this.movingCamera.elapsedTime >= this.movingCamera.movingTime) {
                const movingCamera = this.movingCamera;
                this.movingCamera = null;

                cameraOptions.position = [...movingCamera.targetCameraOptions.position];
                cameraOptions.quaternion = movingCamera.targetCameraOptions.quaternion === null ? null : [...movingCamera.targetCameraOptions.quaternion];
                cameraOptions.rotation = [...movingCamera.targetCameraOptions.rotation];

                CameraManager.setCamera(this.camera, this.controls, cameraOptions);
                
                this.postMoveCamera(movingCamera.mode, movingCamera.fov, movingCamera.far, movingCamera.near, movingCamera.param);

                if (this.cameraRotation) {
                    this.cameraRotation[1] = Geometry.getDistance3(this.camera.position.x, this.camera.position.y, this.camera.position.z, this.controls.target.x, this.controls.target.y, this.controls.target.z);
                }

                this.timelog("end camera move");
                this.setState({ loading: false });
            }
            else {
                cameraOptions.position = Geometry.getLinearVertex3(this.movingCamera.beginCameraPos.x, this.movingCamera.beginCameraPos.y, this.movingCamera.beginCameraPos.z, this.movingCamera.targetCameraOptions.position[0], this.movingCamera.targetCameraOptions.position[1], this.movingCamera.targetCameraOptions.position[2], this.movingCamera.distancePosition * this.movingCamera.elapsedTime / this.movingCamera.movingTime);
                cameraOptions.quaternion = this.movingCamera.targetCameraOptions.quaternion === null ? null : Geometry.getLinearVertex4(this.movingCamera.beginCameraQuaternion.x, this.movingCamera.beginCameraQuaternion.y, this.movingCamera.beginCameraQuaternion.z, this.movingCamera.beginCameraQuaternion.w, this.movingCamera.targetCameraOptions.quaternion[0], this.movingCamera.targetCameraOptions.quaternion[1], this.movingCamera.targetCameraOptions.quaternion[2], this.movingCamera.targetCameraOptions.quaternion[3], this.movingCamera.distanceQuaternion * this.movingCamera.elapsedTime / this.movingCamera.movingTime);
                cameraOptions.rotation = Geometry.getLinearVertex3(this.movingCamera.beginCameraRotation.x, this.movingCamera.beginCameraRotation.y, this.movingCamera.beginCameraRotation.z, this.movingCamera.targetCameraOptions.rotation[0], this.movingCamera.targetCameraOptions.rotation[1], this.movingCamera.targetCameraOptions.rotation[2], this.movingCamera.distanceRotation * this.movingCamera.elapsedTime / this.movingCamera.movingTime);

                CameraManager.setCamera(this.camera, this.controls, cameraOptions);
            }
        }
    }

    postMoveCamera(mode, fov, far, near, param) {
        if (mode === Contents3D.Mode_Indoor) {
            if (this.prevIndoorModel) {
                this.prevIndoorModel.visible = true;
            }
        }

        this.camera.fov = fov;
        this.camera.far = far;
        this.camera.near = near;

        this.showOutdoor(mode);

        if (param) {
            if (param.method) {
                param.method(param.methodParam);
            }
        }
    }

    removeBoundingBox() {
        if (this.boundingBoxModel) {
            this.boundingBoxModel.visible = false;
            this.boundingBoxModel = null;
        }
    }

    showOutdoor(mode) {
        this.useBoundingBox = true;
        this.removeBoundingBox();

        if (mode !== Contents3D.Mode_Indoor) {
            //this.textPOIManager.hideEquipZoneSprites();
        }

        const _3dOptions = this.props._3dOptions;
        const outdoorModels = this.outdoorModels;

        if (!_3dOptions || !outdoorModels) {
            return;
        }

        if (mode === Contents3D.Mode_Indoor && outdoorModels) {
            outdoorModels.map(model => {
                model.visible = false;
                return model;
            });
        }
        else {
            if (this.prevIndoorModel) {
                this.prevIndoorModel.visible = false;
                this.prevIndoorModel = null;
            }

            if (this.currentIndoorModel) {
                this.currentIndoorModel.visible = false;
                this.currentIndoorModel = null;
            }

            if (outdoorModels) {
                outdoorModels.map(model => {
                    model.visible = true;
                    return model;
                });

                if (outdoorModels.length > 0) {
                    this.currentModel = outdoorModels[0];
                }
            }

            // 실내 센서들 제거
            this.poiManager.removeSensors(null);
            this.poiManager.addOutdoorSensors(_3dOptions.outdoorZones, _3dOptions.zones, this.props.visibleSensorTypes);

            // 외부에 있는 POI 이동을 했을땐 트리가 접히지 않는다
            /*if (!this.nonChangedStatusInfo) {
                this.props.onChangeBuildingGroup(null, SDMS.SelectedStatusInfoType.none);
                this.nonChangedStatusInfo = false;
            }*/

            if (_3dOptions.indoorModelOnMemory === false) {
                // 실내 모델을 메모리에서 해제한다.
                SpatialManager.clearIndoorModels(this);
            }
        }

        const animationModels = [];

        if (mode !== Contents3D.Mode_Indoor) {
            this.props.setCurrentView(null);

            if (outdoorModels) {
                const outdoorModelCount = outdoorModels.length;

                for (let i = 0; i < outdoorModelCount; i++) {
                    const animationModel = this.modelAnimations[outdoorModels[i].name];

                    if (animationModel) {
                        animationModels.push(animationModel);
                    }
                }
            }
        }
        else {
            if (this.currentIndoorModel) {
                const animationModel = this.modelAnimations[this.currentIndoorModel.name];

                if (animationModel) {
                    animationModels.push(animationModel);
                }
            }
        }

        this.currentAnimationModels = animationModels;
    }

    initViewport = () => {
        if (this.isIndoor()) {
            if (!this.props.currentView) {
                return;
            }

            const buildingID = this.props.currentView.buildingID;
            const zoneID = this.props.currentView.zoneID;

            if ((buildingID === 0 || buildingID) && (zoneID === 0 || zoneID)) {
                const zoneData = this.props._3dOptions.zones[zoneID];

                if (zoneData) {
                    const floorIndex = zoneData[0];
                    this.currentIndoorModel = null;
                    this.moveToFloor(buildingID, floorIndex);
                }
            }
        }
        else {
            this.props.initOutdoorViewport();
        }
    }

    // 현재 View를 기본 View로 만든다.
    setInitialViewport = () => {
        if (this.isIndoor()) {

        }
        else {
            CameraManager.setCameraOptions(this.camera, this.controls, this.props._3dOptions?.outdoorModel?.camera);
        }
    }

    moveToFloor = (buildingID, floorIndex) => {
        const building = this.props._3dOptions.buildingIDs[buildingID.toString()];

        if (building) {
            const buildingGroupName = building[1];
            const buildingName = building[2];

            const buildingGroup = this.getBuildingGroupIndoorModel(buildingGroupName, buildingName);
            
            if (buildingGroup) {
                let buildingData = buildingGroup[buildingName];

                if (!buildingData) {
                    buildingData = this.getBuildingDataFromDisplayText(buildingName, buildingGroup);
                }

                if (buildingData && buildingData.floors) {
                    for (let i = 0; i < buildingData.floors.length; i++) {
                        const floor = buildingData.floors[i];

                        if (floor.floorIndex === floorIndex) {
                            if (floor.file) {
                                if (this.isEditMode()) {
                                    //this.showIndoorOrtho(floor.file, floor.cameraOrtho, floor.zoneID);
                                }
                                else {
                                    this.showIndoor(floor.file, floor.modelDisplayText, floor.camera, floor.zoneID);
                                }
                                return floor.zoneID;
                            }
                        }
                    }
                }

                // 층이동에 실패하면 건물로 이동한다.
                if (buildingData && buildingData.file && buildingData.camera) {
                    this.moveToBuilding(buildingName);
                    return -1;
                }

                // 건물로 이동하는 것에도 실패하면 건물그룹으로 이동한다.
                this.moveToBuildingGroup(buildingGroupName);
            }
        }

        return -1;
    }

    showIndoor(modelFile, modelDescription, cameraOptions, zoneID) {
        if (this.prevIndoorModel && this.prevIndoorModel.name === modelFile && this.currentIndoorModel && this.currentIndoorModel.name === modelFile) {
            if (Geometry.getDistance3(this.camera.position.x, this.camera.position.y, this.camera.position.z, cameraOptions.position[0], cameraOptions.position[1], cameraOptions.position[2]) > Geometry.Tolerance ||
                Geometry.getDistance3(this.camera.rotation.x, this.camera.rotation.y, this.camera.rotation.z, cameraOptions.rotation[0], cameraOptions.rotation[1], cameraOptions.rotation[2]) > Geometry.Tolerance) {
                const param = {
                    method: Contents3D.showZoneSensors,
                    methodParam: {
                        contents3D: this,
                        zoneID: zoneID
                    }
                };
                this.setMovingCamera(cameraOptions, Contents3D.Mode_Indoor, param);
            }

            return true;
        }

        const _3dOptions = this.props._3dOptions;
        
        //this.setSelectedFacility(null);
        const zoneData = _3dOptions.zones[zoneID];

        if (zoneData) {
            if (zoneData.length >= 2) {
                //const buildingID = zoneData[1];
                //const buildingData = _3dOptions.buildingIDs[buildingID];

                /*if (buildingData && buildingData.length >= 4) {
                    const modelName = buildingData[3];
                    DataInfo.processBuildingData(modelName, this.props.showBuildingInfo);
                }*/
            }
        }

        // 카메라 이동이 끝난후 나타나게 한다.
        /*this.textPOIManager.hideEquipZoneSprites();

        this.textPOIManager.updateIndoorDatas(zoneID, _3dOptions, this.poiManager);*/

        if (this.prevIndoorFacility) {
            this.prevIndoorFacility.object.visible = false;
            this.prevIndoorFacility = null;
        }

        const modelData = this.internalModels[modelFile];

        if (modelData) {
            // 카메라 이동이 끝난후 나타나게 한다.
            this.poiManager.removeSensors(null);

            const model = modelData[0];

            if (this.prevIndoorModel) {
                this.prevIndoorModel.visible = false;
            }

            const param = {
                method: Contents3D.showZoneSensors,
                methodParam: {
                    contents3D: this,
                    zoneID: zoneID
                }
            };

            model.visible = true;
            this.prevIndoorModel = model;
            this.currentIndoorModel = model;
            this.currentModel = model;

            this.setMovingCamera(cameraOptions, Contents3D.Mode_Indoor, param);
            //this.setMovingCamera(modelData[1], Contents3D.Mode_Indoor, param);
            const value = this.props.getSpatialInfo(param.zoneID);
            if (value && value.length === 3) {
                this.props.onChangeBuildingGroup(value[0], SpaceBody.SelectedStatusInfoType.buildingGroup);
                this.props.onChangeBuildingGroup(value[1], SpaceBody.SelectedStatusInfoType.building);
                this.props.onChangeBuildingGroup(value[2], SpaceBody.SelectedStatusInfoType.zone);
            }

            const exitArrow = modelData[2];

            if (exitArrow !== null) {
                exitArrow.visible = false;
                //this.addBlink(exitArrow, 1.5, 1);
            }

            this.props.setCurrentView(zoneID);
            this.useBoundingBox = false;
            return true;
        }
        else if (_3dOptions?.indoorModelOnMemory === false) {
            SpatialManager.showIndoor(zoneID, _3dOptions, this);
            return true;
        }

        return false;
    }

    static showZoneSensors(param) {
        const contents3D = param.contents3D;

        if (param.zoneID !== null && param.zoneID !== undefined) {
            contents3D.poiManager.addZoneSensors(param.zoneID, POIManager.IndoorPoiScale, contents3D.props._3dOptions.outdoorZones, contents3D.props._3dOptions.zones, contents3D.state.visibleSensorTypes);

            /*contents3D.textPOIManager.hideEquipZoneSprites();
            contents3D.textPOIManager.showEquipZoneSprites(param.zoneID);
            contents3D.fakeWallManager.showFakeWalls();*/

            if (param.sensorType && param.sensorID !== null && param.sensorID !== undefined) {
                contents3D.moveToSensor(param.zoneID, param.sensorType, param.sensorID);
            }
        }
    }

    getBuildingGroupIndoorModel(buildingGroupName, buildingName) {
        const indoorModels = this.props._3dOptions.indoorModels;

        if (indoorModels) {
            const buildingGroup = indoorModels[buildingGroupName];

            if (buildingGroup) {
                let buildingData = buildingGroup[buildingName];

                if (!buildingData) {
                    buildingData = this.getBuildingDataFromDisplayText(buildingName, buildingGroup);
                }

                if (buildingData) {
                    return buildingGroup;
                }
            }

            for (const name in indoorModels) {
                const bg = indoorModels[name];

                if (bg) {
                    let buildingData = bg[buildingName];

                    if (!buildingData) {
                        buildingData = this.getBuildingDataFromDisplayText(buildingName, bg);
                    }

                    if (buildingData) {
                        return bg;
                    }
                }
            }
        }

        return null;
    }

    getBuildingDataFromDisplayText(displayText, buildingGroup) {
        for (const buildingName in buildingGroup) {
            const buildingData = buildingGroup[buildingName];

            if (buildingData.modelDisplayText === displayText) {
                return buildingData;
            }
        }

        return null;
    }

    moveToBuilding(buildingName) {
        const building = this.props._3dOptions.allBuildings[buildingName];

        if (building) {
            const buildingGroupName = building[1];
            const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

            if (buildingGroup) {
                const buildingModel = buildingGroup[buildingName];

                if (buildingModel && buildingModel.camera) {
                    this.setMovingCamera(buildingModel.camera, Contents3D.Mode_Outdoor_Part, null);
                }
            }
        }
    }

    isEditMode() {
        return this.camera === this.orthoGraphicCamera;
    }

    getSelectedSensor() {
        const sensors = [...this.props.selectedNodes];

        if (sensors.length === 0) {
            return null;
        }

        return sensors[0];
    }

    screenToGlobal(event) {
        const x = event.nativeEvent.offsetX;
        const y = event.nativeEvent.offsetY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, this.camera);
        return [raycaster.ray.origin.x, raycaster.ray.origin.z];
    }

    onMouseMove = (event) => {
        if (this.state.loading === false && this.camera && this.isEditMode()) {
            const selectedSensor = this.getSelectedSensor();

            if (selectedSensor && this.poiManager.CurrentMode === POIEdit.Menu_Add && POIManager.hasNoPosition(selectedSensor)) {
                const [x, z] = this.screenToGlobal(event);
                const y = this.getDefaultPoiElevation();
                this.poiManager.showTempPOI(this.isIndoor(), true, selectedSensor, x, y, z);
            }
            else if (this.poiManager.selectedPOI && this.poiManager.CurrentMode === POIEdit.Menu_Move) {
                const [x, z] = this.screenToGlobal(event);
                this.poiManager.movePOI(this.poiManager.selectedPOI, x, z);
            }

            return;
        }
    }

    onClick = (event) => {
        if (this.state.loading === false && this.camera && this.isEditMode()) {
            const poi = this.poiManager.getPOI(event, this.camera, false);

            if (this.poiManager.selectedPOI) {
                if (this.poiManager.CurrentMode === POIEdit.Menu_Move) {
                    this.poiManager.putPOI(this.poiManager.selectedPOI, this.props._3dOptions);
                    this.props.onSaveXML(true);
                }
                else if (this.poiManager.CurrentMode === POIEdit.Menu_Delete) {
                    if (poi) {
                        this.poiManager.deletePOI(poi);
                        this.props.onSaveXML(true);
                    }
                }
                else {
                    const selectedSensor = this.getSelectedSensor();

                    if (selectedSensor) {
                        this.poiManager.putTempPOI(this.poiManager.selectedPOI, selectedSensor, this.isIndoor());
                        // Temp XML에 저장
                        this.props.onSaveXML(true);
                    }
                    else if (poi !== this.poiManager.selectedPOI) {
                        this.poiManager.selectPOI(poi, true, null);
                    }
                }
            }
            else {
                this.onClickPOI(poi, event);
            }
        }
    }

    onClickPOI(poi, event) {
        if (!poi) {
            this.poiManager.selectPOI(null, this.props.editMode, this.props.editModeParam);
            //this.props.onSelectPOI(null, false, this);
            return;
        }

        if (this.poiManager.CurrentMode === POIEdit.Menu_Delete) {
            this.poiManager.deletePOI(poi);
            this.props.onSaveXML(true);
            return;
        }

        let poiName = poi.object.name;
        if (poiName === undefined || poiName === "")
            return;

        let type = this.poiManager.getSensorType(poiName);
        if (type === null)
            return;

        let id = this.poiManager.getSensorID(poiName);
        if (id === null)
            return;

        // 타입에 따라 기능 구별
        /*if (type === SDMSMainMenu.CCTV_Type || type === SDMSMainMenu.CCTV_SafetyI_Type || type === SDMSMainMenu.CCTV_PTZ_Type ||
            (this.props.editMode === Contents3D.Edit_Mode_CCTVGroup && this.props.editModeParam === CCTVInfo.Mode_Select_CCTV)) {

            if (type === SDMSMainMenu.CCTV_Type || type === SDMSMainMenu.CCTV_SafetyI_Type || type === SDMSMainMenu.CCTV_PTZ_Type) {
                // CCTV ID 전달하기
                this.props.onSelectCCTV(id, poi, this.poiManager);
            }
            else {
                return;
            }
        }*/

        this.clickForMovePOI(poi, event);
    }

    clickForMovePOI(poi, event) {
        if (this.props.editMode === Contents3D.Edit_Mode_MovePOI ||
            this.props.editMode === Contents3D.Edit_Mode_Text) {
            if (this.pickPOI) {
                this.pickPOI = null;
            }
            /*else {
                if (this.props.editModeManager.movePoiMode()) {
                    this.pickPOI = poi;
                    // 아직 센서 위치가 정해지지 않았지만 일단 원래 위치에서 이동했기 때문에 편집되었다고 알려준다.
                    this.props.onSelectPOI(poi, true, this);

                    if (this.doesClickEquipZoneTextInEditTextMode(poi)) {
                        this.setEditableInput(poi, true, event);
                        //this.setState({ editableInput: true });
                    }
                }
                else if (this.props.editModeManager.deletePoiMode()) {
                    this.pickPOI = null;

                    if (this.poiManager.deleteCCTV(poi, this.props._3dOptions)) {
                        this.props.onDeleteCCTV(poi, this.poiManager);
                    }

                    return;
                }
                else if (this.props.editModeManager.checkNDeletePoiMode()) {
                    this.pickPOI = null;
                    const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(poi);

                    if (sensorType && sensorID !== null) {
                        this.showBuildingInfo(sensorType, sensorID);
                    }

                    if (sensorType === SDMSMainMenu.CCTV_Type ||
                        sensorType === SDMSMainMenu.CCTV_SafetyI_Type) {
                        const cctv = SDMSDataManager.getSensor(sensorType, zoneID, sensorID, this.props._3dOptions);

                        if (cctv) {
                            this.confirmDialogData = poi;
                            const message = [`선택한 cctv(${cctv.id}.${cctv.cameraName})를 삭제하시겠습니까?`];
                            this.props.showConfirmDialog("확인", message, ["예", "아니오"], this.onClickDeleteCCTVYesNo);
                        }
                    }

                    return;
                }
                else if (this.props.editModeManager.checkPoiMode()) {
                    this.pickPOI = null;
                    const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(poi);

                    if (sensorType && sensorID !== null) {
                        this.showBuildingInfo(sensorType, sensorID);
                    }
                    return;
                }
            }*/
        }

        this.poiManager.selectPOI(poi, this.props.editMode, this.props.editModeParam);
        //this.props.onSelectPOI(poi, false, this);
    }

    getDefaultPoiElevation() {
        if (this.isIndoor()) {
            const currentView = this.props.currentView;

            if (currentView?.zoneID) {
                const cameraOptions = CameraManager.getIndoorModelCamera(SpaceBody.Type_Floor, currentView.zoneID, this.props._3dOptions, true);
                return cameraOptions.targetControl[1] + 10;
            }
        }
        else {
            const cameraOptions = CameraManager.getOutdoorModelCamera(this.props._3dOptions, true);
            return cameraOptions.targetControl[1] + 10;
        }

        return 0;
    }

    render() {
        const className = this.state.loading ? styles.contents3DArea + " " + styles.loading : styles.contents3DArea;
        const [currentBuildingID, currentFloorDatas] = [null, null];//this.getCurrentBuildingFloors();
        
        return (
               <main className={styles.appWrap}> 
                {
                    (this.state.progressActive) ?
                        <ProgressBar active={this.state.progressActive} progress={this.state.progressValue} />
                        : null
                }
                <Toolbar
                    initViewport={this.initViewport}
                    setInitialViewport={this.setInitialViewport}
                    buildingID={currentBuildingID}
                    floorDatas={currentFloorDatas}
                    moveToFloor={this.moveToFloor}
                />
                <section className={styles.appContainerWrap + " " + styles.clfix}>
                    <div ref={this.ref3D} className={className} onClick={this.onClick} onMouseMove={this.onMouseMove}>
                    </div>

                    {/* <div className={styles.ViewSet}>
                        <span className={styles.basicViewSet}>기본뷰로 설정</span>
                        <span className={styles.illuminanceSet}>조도 설정</span>
                    </div>

                    <div className={styles.lightBox}>
                        <span className={styles.directLightText}>직사광</span>
                        <div className={styles.lightwrapper}>
                            <div className={styles.inner}>
                                <div className={styles.percentage}>0%</div>
                                <div className={styles.progressBar}>
                                    <div className={styles.moveMe}>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <span className={styles.dispersedLightText}>분산광</span>
                        <div className={styles.lightwrapper2}>
                            <div className={styles.inner2}>
                                <div className={styles.percentage2}>0%</div>
                                <div className={styles.progressBar2}>
                                    <div className={styles.moveMe2}>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div> */}

                </section>
            </main>
        );
    }
}
