import * as React from 'react';
import * as THREE from "three/build/three.module.js";
import { OrbitControls } from "three/examples/jsm/controls/OrbitControls.js";
import { GLTFLoader } from "three/examples/jsm/loaders/GLTFLoader.js";
import { DRACOLoader } from "three/examples/jsm/loaders/DRACOLoader.js";
import { FBXLoader } from "three/examples/jsm/loaders/FBXLoader.js";
import styles from '../../css/sdms.module.css';
import * as Frontend from '../../data/frontend';
import { SDMSDataManager } from '../../services/sdmsDataManager';
import { SDMSController } from '../../services/sdmsController';
import Geometry from '../../../Common/data/Geometry';
import * as Common from '../../../Common/data/common';
import SDMSMainMenu from '../../data/sdmsMainMenu';
import TextPOIManager from './textPOIManager';
import POIManager from './poiManager';
import $ from 'jquery';
import ProjectResource from '../../../Root/resource/id';

/*interface Props {
    _3dOptions: Frontend._3DOptions,
    setCurrentView: (zoneID: Common.NullableNumber) => void
}

interface State {
    loading: boolean,
    prevInstance: Contents3D,
    prevProps: Props
}*/

class Contents3D extends React.Component/*<Props, State>*/ {
    static Mode_Outdoor = 0;
    static Mode_Indoor = 2;

    static Alarm_Model = ["", "Alarm_Level1.glb", "Alarm_Level2.glb", "Alarm_Level3.glb"];
    static NO_ALARM = 0;
    // 관심
    static ALARM_1 = 1;
    // 주의
    static ALARM_2 = 2;
    // 경계
    static ALARM_3 = 3;
    // 심각
    static ALARM_4 = 4;

    static FacilityHeadTag = "equipment-";
    static OutdoorFacilityHeadTag = "out-equipment";

    static PipeGroup_Tag = "plumbing-";
    static PipeEquipmentGroup_Tag = "plant-";
    static FloorGroup_Tag = "bottom-";

    static FacilityModelFileName = "nst-plant.glb";

    /*private ref3D: React.RefObject<HTMLDivElement> = React.createRef();;

    private clock: THREE.Clock = new THREE.Clock();
    private renderer: THREE.WebGLRenderer = null;
    private scene: THREE.Scene = null;
    private camera: THREE.Camera = null;
    private dirLight: THREE.DirectionalLight = null;
    private controls: OrbitControls = null;
    private currentModel: THREE.Object3D = null;

    private orthoGraphicCamera: THREE.OrthographicCamera = null;
    private perspectiveCamera: THREE.PerspectiveCamera = null;

    private perspectiveControlOrigin: THREE.Vector3 = null;

    private outdoorModelCount: number = -1;
    private completeOutdoorModelCount: number = -1;

    private outdoorModels: THREE.Object3D[] = [];
    private internalModels: Map<string, Frontend.InternalModel> = new Map<string, Frontend.InternalModel>();

    private useBoundingBox: boolean = true;
    private boundingBoxModel: THREE.Object3D = null;

    private currentIndoorModel: THREE.Object3D = null;
    private prevIndoorModel: THREE.Object3D = null;

    private movingCamera: Frontend.MovingCamera | null = null;

    // 배관
    private pipeLayer: THREE.Object3D = null;
    //private pipeParents: Map<THREE.Object3D, THREE.Object3D> = new Map<THREE.Object3D, THREE.Object3D>();
    // 배관에 연결된 설비들
    private pipeEquipmentLayer: THREE.Object3D = null;
    //private pipeEquipmentParents: Map<THREE.Object3D, THREE.Object3D> = new Map<THREE.Object3D, THREE.Object3D>();
    private pipeFloorLayer: THREE.Object3D = null;
    private offObjects: THREE.Object3D[] = [];

    private textPOIManager = new TextPOIManager();
    private poiManager = new POIManager();

    private resizeMethod: () => any;*/

    constructor(props/*: Props*/) {
        super(props);

        this.state =
        {
            loading: false,
            prevInstance: this,
            alarm: Contents3D.NO_ALARM,
            prevProps: props
        };

        this.ref3D = React.createRef();
        this.refTooltip = React.createRef();

        this.clock = new THREE.Clock();
        this.renderer = null;
        this.scene = null;
        this.camera = null;
        this.dirLight = null;
        this.controls = null;
        this.currentModel = null;

        this.orthoGraphicCamera = null;
        this.perspectiveCamera = null;

        this.perspectiveControlOrigin = null;

        this.outdoorModelCount = -1;
        this.completeOutdoorModelCount = -1;

        this.outdoorModels = [];
        this.internalModels = {};

        this.useBoundingBox = true;
        this.boundingBoxModel = null;

        this.currentIndoorModel = null;
        this.prevIndoorModel = null;

        this.movingCamera = null;

        // 배관
        this.pipeLayer = null;
        //private pipeParents: Map<THREE.Object3D, THREE.Object3D> = new Map<THREE.Object3D, THREE.Object3D>();
        // 배관에 연결된 설비들
        this.pipeEquipmentLayer = null;
        this.pipeFloorLayer = null;
        this.offObjects = [];

        this.textPOIManager = new TextPOIManager();
        this.poiManager = new POIManager(this);

        this.alarmAnimationMixers = [[], [], [], []];
        this.alarmModels = [[], [], [], []];

        this.tempVisibleSensorTypes = {};
        this.tooltipText = "";
        this.visibleTooltip = false;
    }

    componentDidMount() {
        if (this.props._3dOptions.outdoorModel) {
            this.init();
            Contents3D.animate(this);

            const modelFiles = this.getOutdoorModelFiles();
            this.loadOutdoorModelFiles(modelFiles);
            //this.loadTestModel('NST-out.glb');

            this.resizeMethod = () => Contents3D.onWindowResize(this.camera, this.renderer);

            window.addEventListener('resize', this.resizeMethod, false);
            window.addEventListener('keydown', this.onKeyDown, false);

            if (this.props.workers) {
                for (const zoneID in this.props._3dOptions.outdoorZones) {
                    this.poiManager.setWorkerIcons(this.props.workers, zoneID);
                    break;
                }
            }
        }
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.resizeMethod);
        window.removeEventListener('keydown', this.onKeyDown);
        this.detach3D();
    }

    getOutdoorModelFiles()/*: string[]*/ {
        const modelFiles = [];
        //const modelFiles = new Array<string>();
        modelFiles.push(this.props._3dOptions.outdoorModel.file);

        for (const buildingGroupName in this.props._3dOptions.indoorModels) {
            const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

            if (buildingGroup.file) {
                modelFiles.push(buildingGroup.file);
            }
        }

        return modelFiles;
    }

    static getDerivedStateFromProps(props/*: Props*/, state/*: State*/)/*: State*/ {
        if (props === state.prevProps) {
            return state;
        }

        Contents3D.processMenu(props, state);

        return {
            loading: state.loading,
            prevInstance: state.prevInstance,
            prevProps: props
        };
    }

    static processMenu(props, state) {
        if (props.command) {
            if (props.command.menu == SDMSMainMenu.Menu_Save_BuildingGroup_Viewport) {
                state.prevInstance.saveViewport(props.command.menuParameter, props._3dOptions.indoorModels, props.command.menuParameter, null, null);
                state.prevInstance.changeCamera(false);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Save_Building_Viewport) {
                state.prevInstance.saveViewport(props.command.menuParameter, props._3dOptions.indoorModels, null, props.command.menuParameter, null);
                state.prevInstance.changeCamera(false);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Move_EquipZoneName) {
                const [zoneID, equipZoneID, equipZoneName, x, y, z] = props.command.menuParameter;
                state.prevInstance.textPOIManager.moveEquipZoneNameText(zoneID, equipZoneID, equipZoneName, x, y, z, state.prevInstance.postMoveEquipZoneNameText);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Move_Sensor) {
                const [sensorType, sensorID, zoneID, x, y, z] = props.command.menuParameter;
                state.prevInstance.poiManager.moveSensor(sensorType, sensorID, zoneID, x, y, z);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Add_Sensors) {
                const [sensorType, sensors, zoneID] = props.command.menuParameter;
                state.prevInstance.poiManager.removeSensors(null);
                state.prevInstance.poiManager.addSensors(sensorType, sensors, 1, zoneID);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Show_Alarm) {
                const [zoneID, sensorType, sensorID, alarmLevel, isAlarm] = props.command.menuParameter;
                state.prevInstance.showAlarm(zoneID, sensorType, sensorID, alarmLevel, isAlarm);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Hide_Alarm) {
                state.prevInstance.hideAlarms();
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Add_Alarm) {
                const [zoneID, sensorType, sensorID, alarmLevel] = props.command.menuParameter;
                state.prevInstance.addAlarm(zoneID, sensorType, sensorID, alarmLevel);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Remove_Alarm) {
                const [sensorType, sensorID, alarmLevel] = props.command.menuParameter;
                state.prevInstance.removeAlarm(sensorType, sensorID, alarmLevel);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_MoveTo_BuildingGroup) {
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

                state.prevInstance.visibleTooltip = false;
                state.prevInstance.tooltipText = '';
            }
            else if (props.command.menu == SDMSMainMenu.Menu_MoveTo_Building) {
                const buildingName = props.command.menuParameter;
                state.prevInstance.moveToBuilding(buildingName);
                state.prevInstance.visibleTooltip = false;
                state.prevInstance.tooltipText = '';
            }
            else if (props.command.menu == SDMSMainMenu.Menu_MoveTo_POI) {
                //state.prevInstance.hideAlarms();

                const [zoneID, sensorType, sensorID] = props.command.menuParameter;
                state.prevInstance.moveToSensor(zoneID, sensorType, sensorID);
                state.prevInstance.visibleTooltip = false;
                state.prevInstance.tooltipText = '';
            }
            else if (props.command.menu == SDMSMainMenu.Menu_MoveTo_Floor) {
                //state.prevInstance.hideAlarms();

                const [buildingID, floorIndex] = props.command.menuParameter;

                if (buildingID !== undefined && buildingID !== null &&
                    floorIndex !== undefined && floorIndex !== null && floorIndex !== NaN) {
                    state.prevInstance.moveToFloor(buildingID, floorIndex);
                }
                else {
                    props.initOutdoorViewport();
                }

                state.prevInstance.visibleTooltip = false;
                state.prevInstance.tooltipText = '';
            }
            else if (props.command.menu == SDMSMainMenu.Menu_Show_Outdoor) {
                const model = props.command.menuParameter;
                state.prevInstance.setMovingCamera(model.camera, Contents3D.Mode_Outdoor);
                // 건물그룹, 건물의 이름과 좌표를 새로 얻어온다.
                state.prevInstance.textPOIManager.updateOuterDatas(props._3dOptions, state.prevInstance.poiManager);
                state.prevInstance.visibleTooltip = false;
                state.prevInstance.tooltipText = '';
            }
            else if (props.command.menu === SDMSMainMenu.Menu_ClearSelection) {
                state.prevInstance.poiManager.selectPOI(null, props.editMode, props.editModeParam);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_ShowBasicViewMode) {
                state.prevInstance.showPipe(false);
            }
            else if (props.command.menu == SDMSMainMenu.Menu_ShowPipeLineViewMode) {
                state.prevInstance.showPipe(true);
            }

            props.command.menu = SDMSMainMenu.Menu_None;
            props.command.menuParameter = null;
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

        this.renderer = null;
        this.scene = null;
        this.camera = null;
        this.dirLight = null;
        this.controls = null;

        this.internalModels = {}
    }

    init() {
        this.internalModels = {};

        const outdoorModel = this.props._3dOptions.outdoorModel;

        this.orthoGraphicCamera = new THREE.OrthographicCamera(window.innerWidth / - 2, window.innerWidth / 2, window.innerHeight / 2, window.innerHeight / - 2, 0.1, 5000);
        this.perspectiveCamera = new THREE.PerspectiveCamera(outdoorModel.camera.fov, window.innerWidth / window.innerHeight, outdoorModel.camera.near, outdoorModel.camera.far);
        this.camera = this.perspectiveCamera;

        this.scene = new THREE.Scene();
        this.poiManager.Scene = this.scene;

        const bgTexture = new THREE.TextureLoader().load(this.props._3dOptions.textureBaseURL + '/' + this.props._3dOptions.backgroundImage);
        this.scene.background = bgTexture;

        this.dirLight = new THREE.DirectionalLight(0xffffff, 3.0);
        //this.dirLight = new THREE.DirectionalLight(0xffffff, 8.0);
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

        this.scene.add(new THREE.AmbientLight(0x666666, 9));
        //this.scene.add(new THREE.AmbientLight(0x666666, 10));

        this.controls = new OrbitControls(this.camera, this.renderer.domElement);
        this.controls.target.set(0, 0, 0);
        // 최대 회전각
        this.controls.maxPolarAngle = Math.PI / 3;
        this.controls.update();
    }

    static animate(_this/*: Contents3D*/) {
        requestAnimationFrame(() => {
            Contents3D.animate(_this);
        });

        const delta = _this.clock.getDelta();

        if (_this.movingCamera) {
            _this.moveCamera(delta);
        }

        if (_this.renderer && _this.scene && _this.camera) {
            _this.renderer.render(_this.scene, _this.camera);
        }
    }

    moveToSensor(zoneID, sensorType, sensorID) {
        let isIndoor = true;
        let model = null;
        let zone = this.props._3dOptions.zones[zoneID.toString()];

        if (!zone) {
            zone = this.props._3dOptions.outdoorZones[zoneID.toString()];

            if (!zone) {
                return;
            }

            isIndoor = false;
            model = this.props._3dOptions.outdoorModel;
        }
        else {
            model = this.getIndoorZoneModel(zoneID);
        }

        if (zone.sensors && model && model.camera && model.file) {
            const sensors = zone.sensors[sensorType];

            if (sensors) {
                const sensorCount = sensors.length;

                for (let i = 0; i < sensorCount; i++) {
                    const sensor = sensors[i];

                    if (sensor.id === sensorID) {
                        const poi = this.poiManager.getSensorPOI(sensorType, zoneID, sensorID);
                        //const key = this.poiManager.getSensorKey(sensorType, sensorID);
                        //const poi = this.sensorPOIs[key];

                        if (!poi) {
                            this.poiManager.addSensor(sensorType, sensorID, sensor.x, sensor.y, sensor.z, zoneID, isIndoor);
                        }

                        const camera = this.getSensorCameraOption(sensor, model);

                        if (!this.currentModel || this.currentModel.name !== model.file) {
                            if (isIndoor) {
                                this.showIndoor(model.file, /*model.modelDisplayText, */camera, zoneID);
                            }
                            else {
                                this.showOutdoor(Contents3D.Mode_Outdoor);
                                this.setMovingCamera(camera, Contents3D.Mode_Outdoor, null);
                            }
                        }
                        else {
                            if (!isIndoor) {
                                this.setMovingCamera(camera, Contents3D.Mode_Outdoor, null);
                            }
                        }

                        return;
                    }
                }
            }
        }
    }

    getSensorCameraOption(sensor, model) {
        if (sensor.x === null || sensor.x === undefined ||
            sensor.y === null || sensor.y === undefined ||
            sensor.z === null || sensor.z === undefined) {
            return;
        }

        const movePos = [-5.783151245, 32.45516205, 30.26660156];
        const rotation = [-0.9901062846183777, -0.10070063918828964, -0.15202930569648743];

        const camera = {};

        camera.position = [sensor.x + movePos[0], sensor.y + movePos[1], sensor.z + movePos[2]];
        camera.quaternion = null;
        camera.rotation = [...rotation];
        camera.targetControl = [sensor.x, sensor.y, sensor.z];
        //camera.targetControl = [...model.camera.targetControl];
        camera.fov = model.camera.fov;
        camera.near = model.camera.near;
        camera.far = model.camera.far;

        return camera;
    }

    getIndoorZoneModel(zoneID) {
        const zone = this.props._3dOptions.zones[zoneID.toString()];

        if (zone) {
            const buildingID = zone[1];
            const building = this.props._3dOptions.buildingIDs[buildingID.toString()];

            if (building) {
                const buildingGroupName = building[1];
                const buildingName = building[2];

                const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

                if (buildingGroup) {
                    let buildingData = buildingGroup[buildingName];

                    if (!buildingData) {
                        buildingData = this.getBuildingDataFromDisplayText(buildingName, buildingGroup);
                    }

                    if (buildingData && buildingData.floors) {
                        for (let i = 0; i < buildingData.floors.length; i++) {
                            const floor = buildingData.floors[i];

                            if (floor.zoneID === zoneID) {
                                return floor;
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    moveToFloor = (buildingID, floorIndex) => {
        const building = this.props._3dOptions.buildingIDs[buildingID.toString()];

        if (building) {
            const buildingGroupName = building[1];
            const buildingName = building[2];

            const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

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
                                this.showIndoor(floor.file, /*floor.modelDisplayText, */floor.camera, floor.zoneID);
                                return;
                            }
                        }
                    }
                }

                // 층이동에 실패하면 건물로 이동한다.
                if (buildingData && buildingData.file && buildingData.camera) {
                    this.moveToBuilding(buildingName);
                    return;
                }

                // 건물로 이동하는 것에도 실패하면 건물그룹으로 이동한다.
                this.moveToBuildingGroup(buildingGroupName);
            }
        }
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

    showAlarm(zoneID, sensorType, sensorID, alarmLevel, isAlarm) {
        const modelData = SDMSDataManager.getZoneModelData(this.props._3dOptions, zoneID);

        if (!modelData) {
            return;
        }

        this.showIndoor(modelData[0], modelData[1], modelData[2], zoneID);

        const model = this.internalModels[modelData[0]];

        if (model) {
            const exitArrow = model[2];
            const exitArrowDatas = model[3];

            if (exitArrow && exitArrowDatas) {
                this.addMoving(exitArrow, exitArrowDatas, 2);
                //this.addBlink(exitArrow, 1.5, 1);
            }

            let sensorPOI = this.poiManager.getSensorPOI(sensorType, zoneID, sensorID);

            if (sensorPOI) {
                sensorPOI.visible = true;
            }
            else {
                const sensor = this.getSensor(zoneID, sensorType, sensorID);

                if (sensor) {
                    if (sensorType === "etc" || sensorType === "psm") {
                        let sensorPOIData = this.getPOISensor(zoneID, sensorType, sensor.name);

                        if (sensorPOIData) {
                            sensorPOI = this.poiManager.addSensor(sensorType, sensorPOIData.id, sensorPOIData.x, sensorPOIData.y, sensorPOIData.z, zoneID, true);
                        }
                    } else
                        sensorPOI = this.poiManager.addSensor(sensorType, sensorID, sensor.x, sensor.y, sensor.z, zoneID, true);
                }

                if (sensorPOI) {
                    sensorPOI.visible = true;
                }
            }

            if (isAlarm) {
                const alarmModels = this.getCurrentAlarmModels(alarmLevel);

                if (alarmModels && alarmModels.length > 0 && sensorPOI) {
                    this.hideAlarms();

                    const alarmModel = this.moveAlarmAnimation(sensorPOI.position.x, sensorPOI.position.y, sensorPOI.position.z, alarmLevel);

                    if (alarmModel) {
                        alarmModel.visible = true;
                        alarmModel.alarmName = sensorType + "_" + sensorID;
                    }
                }

                this.setState({ alarm: alarmLevel });
            }
        }
    }

    addAlarm(zoneID, sensorType, sensorID, alarmLevel) {
        const modelData = SDMSDataManager.getZoneModelData(this.props._3dOptions, zoneID);

        if (!modelData) {
            return;
        }

        //this.showIndoor(modelData[0], modelData[1], modelData[2], zoneID);

        const model2 = this.internalModels[modelData[0]];

        if (model2) {
            const exitArrow = model2[2];
            const exitArrowDatas = model2[3];

            if (exitArrow && exitArrowDatas) {
                this.addMoving(exitArrow, exitArrowDatas, 2);
                //this.addBlink(exitArrow, 1.5, 1);
            }
        }
        //

        let sensorPOI = this.poiManager.getSensorPOI(sensorType, zoneID, sensorID);

        if (this.currentModel) {
            const model = this.getIndoorZoneModel(zoneID);
            if (model !== null && model.name !== this.currentModel.name) {
                this.hideAlarms();
            }

        }

        if (sensorPOI) {
            sensorPOI.visible = true;
        }
        else {
            const sensor = this.getSensor(zoneID, sensorType, sensorID);

            if (sensor) {
                if (sensorType === "etc" || sensorType === "psm") {
                    let sensorPOIData = this.getPOISensor(zoneID, sensorType, sensor.name);

                    if (sensorPOIData) {
                        sensorPOI = this.poiManager.addSensor(sensorType, sensorPOIData.id, sensorPOIData.x, sensorPOIData.y, sensorPOIData.z, zoneID, true);
                    }
                } else
                    sensorPOI = this.poiManager.addSensor(sensorType, sensorID, sensor.x, sensor.y, sensor.z, zoneID, true);
            }

            if (sensorPOI) {
                sensorPOI.visible = true;
            }
        }

        const alarmModels = this.getCurrentAlarmModels(alarmLevel);

        if (alarmModels && alarmModels.length > 0 && sensorPOI) {
            const alarmModel = this.moveAlarmAnimation(sensorPOI.position.x, sensorPOI.position.y, sensorPOI.position.z, alarmLevel);

            if (alarmModel) {
                alarmModel.visible = true;
                alarmModel.alarmName = sensorType + "_" + sensorID;
            }

            this.setState({ alarm: alarmLevel });
        }
    }

    removeAlarm(sensorType, sensorID, alarmLevel) {
        const alarmModels = this.getCurrentAlarmModels(alarmLevel);

        if (alarmModels) {
            const modelCount = alarmModels.length;
            const target = sensorType + "_" + sensorID;

            for (let i = 0; i < modelCount; i++) {
                const alarmModel = alarmModels[i];

                if (alarmModel && alarmModel.alarmName === target) {
                    alarmModel.visible = false;
                    break;
                }
            }
        }
    }

    getCurrentAlarmModels(alarmLevel) {
        if (alarmLevel > 0) {
            const alarmModels = this.alarmModels[alarmLevel - 1];
            return alarmModels;
        }

        return null;
    }

    hideAlarms() {
        const modelCount = this.alarmModels.length;

        for (let i = 0; i < modelCount; i++) {
            const alarmModels = this.alarmModels[i];
            const alarmModelCount = alarmModels.length;

            for (let j = 0; j < alarmModelCount; j++) {
                const alarmModel = alarmModels[j];

                if (alarmModel && alarmModel.visible) {
                    alarmModel.visible = false;
                }
            }
            /*const alarmModel = this.alarmModels[i];

            if (alarmModel) {
                alarmModel.visible = false;
            }*/
        }

        this.setState({ alarm: Contents3D.NO_ALARM });
    }

    moveAlarmAnimation(x, y, z, alarmLevel) {
        const alarmModels = this.alarmModels[alarmLevel - 1];
        const modelCount = alarmModels.length;

        for (let i = 0; i < modelCount; i++) {
            const alarmModel = alarmModels[i];

            if (alarmModel && alarmModel.visible === false) {
                this.moveAnimationChild(alarmModel, x, y, z);
                return alarmModel;
            }
        }

        return null;
        /*  const modelCount = this.alarmModels.length;

            for (let i = 0; i < modelCount; i++) {
            const alarmModel = this.alarmModels[i];

            if (alarmModel !== null) {
                const childCount = alarmModel.children.length;

                // animation Object는 직접 옮길수 없고 child object들을 모두 옮겨야 한다.
                for (let i = 0; i < childCount; i++) {
                    const childModel = alarmModel.children[i];
                    childModel.position.x = x;
                    childModel.position.y = y;
                    childModel.position.z = z;
                }
            }
        }*/
    }

    moveAnimationChild(model, x, y, z) {
        if (model) {
            const childCount = model.children.length;

            // animation Object는 직접 옮길수 없고 child object들을 모두 옮겨야 한다.
            for (let i = 0; i < childCount; i++) {
                const childModel = model.children[i];
                childModel.position.x = x;
                childModel.position.y = y;
                childModel.position.z = z;
            }
        }
    }

    addMoving(movingGroup, movingChildren, interval) {
        movingGroup.visible = true;

        const moving = {
            models: movingChildren,
            interval: interval,
            delta: 0
        };

        this.movingDatas.push(moving);
    }

    getSensor(zoneID, sensorType, sensorID) {
        let zone = this.props._3dOptions.zones[zoneID];

        if (!zone) {
            zone = this.props._3dOptions.outdoorZones[zoneID];
        }

        if (zone) {
            const sensors = zone.sensors[sensorType];

            if (sensors) {
                const sensorCount = sensors.length;

                for (let i = 0; i < sensorCount; i++) {
                    const sensor = sensors[i];

                    if (sensor.id === sensorID) {
                        return sensor;
                    }
                }
            }
        }

        return null;
    }

    getPOISensor(zoneID, sensorType, sensorName) {
        let zone = this.props._3dOptions.zones[zoneID];

        if (!zone) {
            zone = this.props._3dOptions.outdoorZones[zoneID];
        }

        if (zone) {
            const sensors = zone.sensors[sensorType];

            if (sensors) {
                const sensorCount = sensors.length;

                for (let i = 0; i < sensorCount; i++) {
                    const sensor = sensors[i];

                    if (sensor.name === sensorName && sensor.x !== null && sensor.y !== null && sensor.z !== null) {
                        return sensor;
                    }
                }
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
                    this.setMovingCamera(buildingModel.camera, Contents3D.Mode_Outdoor, null);
                }
            }
        }
    }

    moveToBuildingGroup(buildingGroupName) {
        const modelName = buildingGroupName + SDMSDataManager.BoundingBoxTag;
        const buildingGroup = this.getBuildingGroupModel(modelName);

        if (buildingGroup && buildingGroup.camera) {
            //DataInfo.processBuildingGroupData(buildingGroup.buildingGroupID, this.props.showBuildingInfo);
            this.setMovingCamera(buildingGroup.camera, Contents3D.Mode_Outdoor, null);
        }
    }

    getBuildingGroupModel(boundingBoxName) {
        const buildingGroupCount = this.props._3dOptions.buildingGroups.length;
        let buildingGroupName = null;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = this.props._3dOptions.buildingGroups[i];

            if (buildingGroup[2] === boundingBoxName) {
                buildingGroupName = buildingGroup[0];
                break;
            }
        }

        if (buildingGroupName === null) {
            return null;
        }

        return this.props._3dOptions.indoorModels[buildingGroupName];
    }

    saveViewport(modelName, _3dOptions, buildingGroupName, buildingName, zoneID) {
        const model = this.getModel(modelName, _3dOptions);

        if (model === null) {
            return;
        }

        const hitPoint = this.getRaycastingPosition(window.innerWidth / 2, window.innerHeight / 2);

        const camera/*: Frontend.PerspectiveCameraData*/ = {
            far: model.camera.far,
            fov: model.camera.fov,
            near: model.camera.near,
            position: [this.camera.position.x, this.camera.position.y, this.camera.position.z],
            quaternion: [this.camera.quaternion.x, this.camera.quaternion.y, this.camera.quaternion.z, this.camera.quaternion.w],
            rotation: [this.camera.rotation.x, this.camera.rotation.y, this.camera.rotation.z],
            targetControl: [null, null, null]
        }

        if (hitPoint) {
            camera.targetControl = [hitPoint.x, hitPoint.y, hitPoint.z];
        }
        else {
            camera.targetControl = [this.controls.target.x, this.controls.target.y, this.controls.target.z];
        }

        const buildingGroupID = this.getBuildingGroupID(buildingGroupName);
        const buildingID = this.getBuildingID(buildingName);

        this.requestSaveViewport(model, modelName, model.file, camera, buildingGroupID, buildingID, zoneID);
    }

    getBuildingGroupID(buildingGroupName) {
        if (buildingGroupName) {
            const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

            if (buildingGroup) {
                return buildingGroup.buildingGroupID;
            }
        }

        return null;
    }

    getBuildingID(buildingName) {
        if (buildingName) {
            const building = this.props._3dOptions.allBuildings[buildingName];

            if (building) {
                return building[0];
            }
        }

        return null;
    }

    getRaycastingPosition(x, y) {
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, this.camera);

        const intersects = raycaster.intersectObjects(this.scene.children, true);
        const intersectCount = intersects.length;

        if (intersectCount > 0) {
            return intersects[0].point;
        }

        return null;
    }

    async requestSaveViewport(model, modelName, file, camera, buildingGroupID, buildingID, zoneID) {
        const [success, message] = await SDMSController.requestSaveViewport(modelName, file, camera, model.modelDisplayText, buildingGroupID, buildingID, zoneID);

        if (success) {
            model.camera = camera;
            model.file = file;
            alert("뷰포트 저장 성공");
        }
        else {
            alert(message);
        }
    }

    getModel(modelName, data) {
        for (const key in data) {
            const child = data[key];

            if (key === modelName) {
                return child;
            }

            if (child instanceof Object) {
                const model = this.getModel(modelName, child);

                if (model) {
                    return model;
                }
            }
        }

        return null;
    }

    moveCamera(delta/*: number*/) {
        if (this.movingCamera) {
            const cameraOptions/*: Frontend.PerspectiveCameraData*/ = {
                far: 0,
                fov: 0,
                near: 0,
                position: [null, null, null],
                quaternion: [null, null, null, null],
                rotation: [null, null, null],
                targetControl: [this.movingCamera.targetCameraOptions.targetControl[0], this.movingCamera.targetCameraOptions.targetControl[1], this.movingCamera.targetCameraOptions.targetControl[2]]
            }

            this.movingCamera.elapsedTime += delta;

            if (this.movingCamera.elapsedTime >= this.movingCamera.movingTime) {
                const movingCamera = this.movingCamera;
                this.movingCamera = null;

                cameraOptions.position = [movingCamera.targetCameraOptions.position[0], movingCamera.targetCameraOptions.position[1], movingCamera.targetCameraOptions.position[2]];
                cameraOptions.quaternion = movingCamera.targetCameraOptions.quaternion === null ? null : [movingCamera.targetCameraOptions.quaternion[0], movingCamera.targetCameraOptions.quaternion[1], movingCamera.targetCameraOptions.quaternion[2], movingCamera.targetCameraOptions.quaternion[3]];
                cameraOptions.rotation = [movingCamera.targetCameraOptions.rotation[0], movingCamera.targetCameraOptions.rotation[1], movingCamera.targetCameraOptions.rotation[2]];

                Contents3D.setCamera(this.camera, this.controls, cameraOptions);
                //this.camera.updateProjectionMatrix();

                this.postMoveCamera(movingCamera.mode, movingCamera.fov, movingCamera.far, movingCamera.near, movingCamera.param);

                if (this.cameraRotation) {
                    this.cameraRotation[1] = Geometry.getDistance3(this.camera.position.x, this.camera.position.y, this.camera.position.z, this.controls.target.x, this.controls.target.y, this.controls.target.z);
                }

                this.setState({ loading: false });
            }
            else {
                cameraOptions.position = Geometry.getLinearVertex3(this.movingCamera.beginCameraPos.x, this.movingCamera.beginCameraPos.y, this.movingCamera.beginCameraPos.z, this.movingCamera.targetCameraOptions.position[0]/* as number*/, this.movingCamera.targetCameraOptions.position[1]/* as number*/, this.movingCamera.targetCameraOptions.position[2]/* as number*/, this.movingCamera.distancePosition * this.movingCamera.elapsedTime / this.movingCamera.movingTime);
                cameraOptions.quaternion = this.movingCamera.targetCameraOptions.quaternion === null ? null : Geometry.getLinearVertex4(this.movingCamera.beginCameraQuaternion.x, this.movingCamera.beginCameraQuaternion.y, this.movingCamera.beginCameraQuaternion.z, this.movingCamera.beginCameraQuaternion.w, this.movingCamera.targetCameraOptions.quaternion[0]/* as number*/, this.movingCamera.targetCameraOptions.quaternion[1]/* as number*/, this.movingCamera.targetCameraOptions.quaternion[2]/* as number*/, this.movingCamera.targetCameraOptions.quaternion[3]/* as number*/, (this.movingCamera.distanceQuaternion/* as number*/) * this.movingCamera.elapsedTime / this.movingCamera.movingTime);
                cameraOptions.rotation = Geometry.getLinearVertex3(this.movingCamera.beginCameraRotation.x, this.movingCamera.beginCameraRotation.y, this.movingCamera.beginCameraRotation.z, this.movingCamera.targetCameraOptions.rotation[0]/* as number*/, this.movingCamera.targetCameraOptions.rotation[1]/* as number*/, this.movingCamera.targetCameraOptions.rotation[2]/* as number*/, this.movingCamera.distanceRotation * this.movingCamera.elapsedTime / this.movingCamera.movingTime);

                Contents3D.setCamera(this.camera, this.controls, cameraOptions);
            }
        }
    }

    postMoveCamera(mode/*: number*/, fov/*: number*/, far/*: number*/, near/*: number*/, param/*: any*/) {
        if (mode === Contents3D.Mode_Indoor) {
            this.prevIndoorModel.visible = true;
        }

        if (fov !== 0 || far !== 0 || near !== 0) {
            this.camera.fov = fov;
            this.camera.far = far;
            this.camera.near = near;
        }

        this.showOutdoor(mode);

        if (param) {
            if (param.zoneID !== null && param.zoneID !== undefined) {
                this.poiManager.addZoneSensors(param.zoneID, POIManager.IndoorPoiScale, this.props._3dOptions.outdoorZones, this.props._3dOptions.zones, this.props.visibleSensorTypes);
                this.poiManager.setWorkerIcons(this.props.workers, param.zoneID);
            }
        }
    }

    static setCamera(camera/*: THREE.Camera*/, controls/*: OrbitControls*/, cameraOptions/*: Frontend.PerspectiveCameraData*/) {
        camera.position.set(cameraOptions.position[0], cameraOptions.position[1], cameraOptions.position[2]);

        if (cameraOptions.quaternion) {
            camera.quaternion.set(cameraOptions.quaternion[0], cameraOptions.quaternion[1], cameraOptions.quaternion[2], cameraOptions.quaternion[3]);
        }

        camera.rotation.set(cameraOptions.rotation[0], cameraOptions.rotation[1], cameraOptions.rotation[2]);
        controls.target.set(cameraOptions.targetControl[0], cameraOptions.targetControl[1], cameraOptions.targetControl[2]);

        if (cameraOptions.near !== 0 || cameraOptions.far !== 0 || cameraOptions.fov !== 0) {
            camera.near = cameraOptions.near;
            camera.far = cameraOptions.far;
            camera.fov = cameraOptions.fov;
        }
    }

    toggleCameraMode() {
        if (this.camera === this.perspectiveCamera)
            this.changeCamera(true);
        else
            this.changeCamera(false);
    }

    changeCamera(orthoMode) {
        if (orthoMode) {
            this.camera = this.orthoGraphicCamera;
            this.controls.object = this.camera;
            this.perspectiveControlOrigin = new THREE.Vector3(this.controls.target.x, this.controls.target.y, this.controls.target.z);

            /*const orthoCameraData = this.getCurrentOrthoCameraData();

            if (orthoCameraData) {
                this.camera.position.set(orthoCameraData.position[0], orthoCameraData.position[1], orthoCameraData.position[2]);
                this.camera.rotation.set(orthoCameraData.rotation[0], orthoCameraData.rotation[1], orthoCameraData.rotation[2]);
                this.camera.quaternion.set(orthoCameraData.quaternion[0], orthoCameraData.quaternion[1], orthoCameraData.quaternion[2], orthoCameraData.quaternion[3]);
                this.camera.zoom = orthoCameraData.zoom;
                this.controls.target.set(orthoCameraData.targetControl[0], orthoCameraData.targetControl[1], orthoCameraData.targetControl[2]);

                this.camera.lookAt(this.camera.position.x, this.controls.target.y, this.camera.position.z);
            }
            else*/ {
                this.camera.position.set(this.perspectiveCamera.position.x, this.perspectiveCamera.position.y, this.perspectiveCamera.position.z);
                this.controls.target.set(this.camera.position.x, this.controls.target.y, this.camera.position.z);

                this.camera.lookAt(this.camera.position.x, this.controls.target.y, this.camera.position.z);
            }

            this.camera.up.set(0, 1, 0);
            this.camera.updateProjectionMatrix();
            this.controls.update();

            this.controls.enableRotate = false;
            this.useBoundingBox = false;
        }
        else {
            this.camera = this.perspectiveCamera;
            this.camera.updateProjectionMatrix();
            this.controls.object = this.camera;

            /*const cameraData = this.getCurrentCameraData();

            if (cameraData) {
                this.camera.position.set(cameraData.position[0], cameraData.position[1], cameraData.position[2]);
                this.camera.rotation.set(cameraData.rotation[0], cameraData.rotation[1], cameraData.rotation[2]);
                this.camera.quaternion.set(cameraData.quaternion[0], cameraData.quaternion[1], cameraData.quaternion[2], cameraData.quaternion[3]);
                this.controls.target.set(cameraData.targetControl[0], cameraData.targetControl[1], cameraData.targetControl[2]);
            }
            else*/ {
                this.controls.target.set(this.perspectiveControlOrigin.x, this.perspectiveControlOrigin.y, this.perspectiveControlOrigin.z);
            }

            this.controls.update();

            this.controls.enableRotate = true;
            this.useBoundingBox = true;
        }
    }

    static onWindowResize(camera/*: THREE.Camera*/, renderer/*: THREE.WebGLRenderer*/) {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    onKeyDown = (event/*: KeyboardEvent*/) => {
    }

    loadTestModel(contents/*: string*/) {
        const modelBaseURL = "/resource/gltf";
        const fileName = modelBaseURL + "/" + contents;

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
        //const mode = this.state.mode;
        const cameraOptions = {"position":[-224.2465362548828,302.5075988769531,318.6025390625],"quaternion":[-0.3036808967590332,-0.21341489255428314,-0.06999507546424866,0.9259226322174072],"rotation":[-0.6853181719779968,-0.36045390367507935,-0.2807161211967468],"targetControl":[-56.56464767456055,20.93910789489746,-25.82569122314453],"fov":60,"near":0.1,"far":5000};

        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                child.getWorldPosition(worldPos);
                child.getWorldScale(worldScale);
                child.getWorldQuaternion(worldQuat);

                if (child instanceof THREE.Mesh) {
                    child.castShadow = false;
                    child.receiveShadow = false;
                    worldBox.expandByObject(child);
                }
            });

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;
            _this.scene.add(modelNode);
            modelNode.updateMatrixWorld(true);

            const boxSize = new THREE.Vector3();
            worldBox.getSize(boxSize);

            const sceneMaxLen = boxSize.length();
            const sceneHalfMaxLen = sceneMaxLen * 0.5;

            worldBox.getCenter(_this.dirLight.target.position);
            _this.dirLight.position.copy(_this.dirLight.target.position);

            const lightPos = new THREE.Vector3(sceneHalfMaxLen, sceneMaxLen, sceneHalfMaxLen);
            _this.dirLight.position.add(lightPos);

            const lightDistance = lightPos.length();

            _this.dirLight.shadow.camera.near = lightDistance - sceneHalfMaxLen;
            _this.dirLight.shadow.camera.far = lightDistance + sceneHalfMaxLen;
            _this.dirLight.shadow.camera.right = sceneHalfMaxLen;
            _this.dirLight.shadow.camera.left = -sceneHalfMaxLen;
            _this.dirLight.shadow.camera.top = sceneHalfMaxLen;
            _this.dirLight.shadow.camera.bottom = -sceneHalfMaxLen;
            _this.dirLight.shadow.camera.updateProjectionMatrix();
            _this.renderer.shadowMap.needsUpdate = true;

            Contents3D.setCamera(_this.camera, _this.controls, cameraOptions);

            _this.currentModel = modelNode;
            _this.onCompleteOutdoorModelLoading(modelNode);

            _this.timelog("outdoor model");

            _this.setState({ loading: false });
        });
    }

    loadOutdoorModelFiles(modelFiles/*: string[]*/) {
        this.timelog("Begin Loading");
        const fileCount = modelFiles.length;

        this.outdoorModelCount = fileCount;
        this.completeOutdoorModelCount = 0;

        if (fileCount > 0) {
            this.loadRootModel(modelFiles[0], 1, modelFiles, Contents3D.Mode_Outdoor);
        }
    }

    loadRootModel(contents/*: string*/, nextIndex/*: number*/, files/*: string[]*/, mode/*: number*/) {
        this.setState({ loading: true });

        const fileName = this.props._3dOptions.modelBaseURL + "/" + contents;
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
        //const mode = this.state.mode;
        const cameraOptions = this.props._3dOptions.outdoorModel.camera;

        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                child.getWorldPosition(worldPos);
                child.getWorldScale(worldScale);
                child.getWorldQuaternion(worldQuat);

                if (child instanceof THREE.Mesh) {
                    child.castShadow = false;
                    child.receiveShadow = false;
                    worldBox.expandByObject(child);
                }
            });

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;
            _this.scene.add(modelNode);
            modelNode.updateMatrixWorld(true);

            //_this.setPipeNEquipments(modelNode);
            Contents3D.hideBoundingBoxes(modelNode);

            const boxSize = new THREE.Vector3();
            worldBox.getSize(boxSize);

            const sceneMaxLen = boxSize.length();
            const sceneHalfMaxLen = sceneMaxLen * 0.5;

            worldBox.getCenter(_this.dirLight.target.position);
            _this.dirLight.position.copy(_this.dirLight.target.position);

            const lightPos = new THREE.Vector3(sceneHalfMaxLen, sceneMaxLen, sceneHalfMaxLen);
            _this.dirLight.position.add(lightPos);

            const lightDistance = lightPos.length();

            _this.dirLight.shadow.camera.near = lightDistance - sceneHalfMaxLen;
            _this.dirLight.shadow.camera.far = lightDistance + sceneHalfMaxLen;
            _this.dirLight.shadow.camera.right = sceneHalfMaxLen;
            _this.dirLight.shadow.camera.left = -sceneHalfMaxLen;
            _this.dirLight.shadow.camera.top = sceneHalfMaxLen;
            _this.dirLight.shadow.camera.bottom = -sceneHalfMaxLen;
            _this.dirLight.shadow.camera.updateProjectionMatrix();
            _this.renderer.shadowMap.needsUpdate = true;

            Contents3D.setCamera(_this.camera, _this.controls, cameraOptions);

            _this.currentModel = modelNode;
            _this.onCompleteOutdoorModelLoading(modelNode);

            _this.timelog("outdoor model");

            _this.setState({ loading: false });

            if (nextIndex !== null && nextIndex !== undefined && files) {
                if (nextIndex < files.length) {
                    for (let i = nextIndex; i < files.length; i++) {
                        _this.loadFile(files[i], true, null, Contents3D.Mode_Outdoor);
                    }
                }
            }

            _this.poiManager.addOutdoorSensors(_this.props._3dOptions.outdoorZones, _this.props._3dOptions.zones, _this.props.visibleSensorTypes);
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

            if (child.name.endsWith(SDMSDataManager.BoundingBoxTag)) {
                child.visible = false;

                const count = child.children.length;

                for (let j = 0; j < count; j++) {
                    child.children[j].visible = false;
                }
            }
            else {
                Contents3D.hideBoundingBoxes(child);
            }
        }
    }

    loadFile(contents/*: string*/, visible/*: boolean*/, cameraOptions/*: Frontend.PerspectiveCameraData*/, mode/*: number*/) {
        if (visible) {
            this.setState({ loading: true });
        }

        const fileName = this.props._3dOptions.modelBaseURL + "/" + contents;

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
        //const mode = this.state.mode;

        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                /*child.getWorldPosition(worldPos);
                child.getWorldScale(worldScale);
                child.getWorldQuaternion(worldQuat);*/

                if (child instanceof THREE.Mesh) {
                    child.castShadow = false;
                    child.receiveShadow = false;
                }
            });

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;
            modelNode.visible = visible;

            if (_this.scene) {
                _this.scene.add(modelNode);
            }

            modelNode.updateMatrixWorld(true);

            //_this.setPipeNEquipments(modelNode);
            Contents3D.hideBoundingBoxes(modelNode);

            if (visible) {
                _this.onCompleteOutdoorModelLoading(modelNode);
            }
            else {
                _this.internalModels[contents] = [modelNode, cameraOptions, null, null];

                const facilityGroup = Contents3D.showFacilities(modelNode, false);
                _this.internalModels[contents].push(facilityGroup);
            }

            _this.timelog(contents);
            _this.setState({ loading: false });
        });
    }

    static showFacilities(modelNode, visible) {
        const childCount = modelNode.children.length;

        if (modelNode.name.startsWith(Contents3D.FacilityHeadTag) && modelNode.name.endsWith(SDMSDataManager.BoundingBoxTag)) {
            for (let i = 0; i < childCount; i++) {
                const child = modelNode.children[i];

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
            const child = Contents3D.showFacilities(modelNode.children[i], visible);

            if (child !== null) {
                return child;
            }
        }

        return null;
    }

    setPipeNEquipments(model/*: THREE.Object3D*/) {
        if (this.pipeLayer === null) {
            this.pipeLayer = new THREE.Object3D();
            this.scene.add(this.pipeLayer);
            this.pipeLayer.visible = false;
        }

        if (this.pipeEquipmentLayer === null) {
            this.pipeEquipmentLayer = new THREE.Object3D();
            this.scene.add(this.pipeEquipmentLayer);
            this.pipeEquipmentLayer.visible = false;
        }

        if (this.pipeFloorLayer === null) {
            this.pipeFloorLayer = new THREE.Object3D();
            this.scene.add(this.pipeFloorLayer);
            this.pipeFloorLayer.visible = false;
        }

        if (model.name.startsWith(Contents3D.PipeGroup_Tag)) {
            this.pipeLayer.add(model.clone());
            return;
        }

        if (model.name.startsWith(Contents3D.PipeEquipmentGroup_Tag)) {
            this.pipeEquipmentLayer.add(model.clone());
            return;
        }

        if (model.name.startsWith(Contents3D.FloorGroup_Tag)) {
            this.pipeFloorLayer.add(model.clone());
            return;
        }

        const childCount = model.children.length;

        for (let i = 0; i < childCount; i++) {
            this.setPipeNEquipments(model.children[i]);
        }
    }

    // 실내공간 로딩
    async loadIndoorModels() {
        for (const buildingGroupName in this.props._3dOptions.indoorModels) {
            const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

            for (const buildingName in buildingGroup) {
                const building = buildingGroup[buildingName];

                if (building && building.floors) {
                    const floorCount = building.floors.length;

                    for (let i = 0; i < floorCount; i++) {
                        const floor = building.floors[i];

                        if (floor.file && floor.camera) {
                            //this.addEquipZoneText(floor.zoneID);
                            this.loadFile(floor.file, false, floor.camera, Contents3D.Mode_Indoor);
                        }
                    }
                }
            }
        }
    }

    // 배관 및 설비 파일 열기
    async loadFacilityModels(contents/*: string*/, mode/*: number*/) {
        const fileName = this.props._3dOptions.modelBaseURL + "/" + contents;
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

        // 외부모델파일의 카메라를 같이 쓴다.
        const cameraOptions = this.props._3dOptions.outdoorModel.camera;

        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                child.getWorldPosition(worldPos);
                child.getWorldScale(worldScale);
                child.getWorldQuaternion(worldQuat);

                if (child instanceof THREE.Mesh) {
                    child.castShadow = false;
                    child.receiveShadow = false;
                    worldBox.expandByObject(child);
                }
            });

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;

            _this.setPipeNEquipments(modelNode);
            /*_this.facilityLayer = new THREE.Object3D();
            _this.facilityLayer.visible = false;
            _this.facilityLayer.add(modelNode);

            _this.scene.add(_this.facilityLayer);*/
            modelNode.updateMatrixWorld(true);
            _this.timelog("facility model");
        });
    }

    onCompleteOutdoorModelLoading(modelNode/*: THREE.Object3D*/) {
        this.completeOutdoorModelCount = this.completeOutdoorModelCount + 1;

        if (this.completeOutdoorModelCount >= this.outdoorModelCount) {
            this.loadAlarmModels();
            this.loadIndoorModels();
            this.loadFacilityModels(Contents3D.FacilityModelFileName, Contents3D.Mode_Outdoor);
        }

        this.outdoorModels.push(modelNode);
    }

    timelog(log/*: string*/) {
        const now = new Date();
        const time = now.getMinutes() + ":" + now.getSeconds();
        console.log(time + " : " + log);
    }

    removeBoundingBox() {
        if (this.boundingBoxModel) {
            this.boundingBoxModel.visible = false;
            this.boundingBoxModel = null;
        }
    }

    async loadAlarmModels() {
        this.loadAnimationFile(Contents3D.Alarm_Model[Contents3D.ALARM_2 - 1], false, Contents3D.ALARM_2);
        this.loadAnimationFile(Contents3D.Alarm_Model[Contents3D.ALARM_3 - 1], false, Contents3D.ALARM_3);
        this.loadAnimationFile(Contents3D.Alarm_Model[Contents3D.ALARM_4 - 1], false, Contents3D.ALARM_4);
    }
    loadAnimationFile(contents, visible, alarmLevel) {
        const fileName = this.props._3dOptions.modelBaseURL + "/" + contents;

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

        loader.load(fileName, function (object) {
            const obj = loader instanceof GLTFLoader ? object.scene : object;
            obj.traverse((child) => {
                if (child instanceof THREE.Mesh) {
                    child.castShadow = false;
                    child.receiveShadow = false;
                }
            });

            const modelNode = new THREE.Object3D();
            modelNode.add(obj);
            modelNode.matrixAutoUpdate = false;
            modelNode.name = contents;
            modelNode.visible = visible;

            _this.scene.add(modelNode);
            modelNode.updateMatrixWorld(true);

            if (object.animations.length > 0) {
                const childCount = modelNode.children.length;

                // animation Object는 직접 옮길수 없고 child object들을 모두 옮겨야 한다.
                for (let i = 0; i < childCount; i++) {
                    const childModel = modelNode.children[i];
                    childModel.position.y += 10;
                    childModel.scale.x *= 0.20;
                    childModel.scale.z *= 0.20;

                    const meshCount = childModel.children.length;

                    // animation Object에 의한 그림자가 생기는것을 차단한다.
                    for (let j = 0; j < meshCount; j++) {
                        const childMesh = childModel.children[j];
                        childMesh.castShadow = false;
                        childMesh.receiveShadow = false;
                    }
                }

                const mixer = new THREE.AnimationMixer(modelNode);

                for (let i = 0; i < object.animations.length; i++) {
                    mixer.clipAction(object.animations[i]).play();;
                }

                //_this.alarmAnimationMixers[alarmLevel - 1] = mixer;
                //_this.alarmModels[alarmLevel - 1] = modelNode;

                _this.alarmAnimationMixers[alarmLevel - 1].push(mixer);
                _this.alarmModels[alarmLevel - 1].push(modelNode);

                for (let i = 0; i < 99; i++) {
                    const cloneModel = modelNode.clone();
                    _this.alarmModels[alarmLevel - 1].push(cloneModel);
                    _this.scene.add(cloneModel);

                    const mixer2 = new THREE.AnimationMixer(cloneModel);

                    for (let j = 0; j < object.animations.length; j++) {
                        mixer2.clipAction(object.animations[j]).play();;
                    }

                    _this.alarmAnimationMixers[alarmLevel - 1].push(mixer2);
                }

                modelNode.updateMatrixWorld(true);

                _this.timelog(contents);
            }
        });
    }

    isIndoor()/*: boolean*/ {
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

    getBuildingModel(buildingName/*: string*/)/*: Array<Frontend.ZoneModel> | null*/ {
        const building = this.props._3dOptions.allBuildings[buildingName];

        if (building) {
            const buildingGroupName = building[1];
            const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

            if (buildingGroup) {
                const buildingData = buildingGroup[buildingName];

                if (buildingData && buildingData.floors) {
                    return buildingData.floors;
                }
            }
        }

        return null;
    }

    getIndoorModel(boundingBoxName/*: string*/)/*: Frontend.ZoneModel | null*/ {
        const buildingName = boundingBoxName.substring(0, boundingBoxName.length - SDMSDataManager.BoundingBoxTag.length);
        const floors = this.getBuildingModel(buildingName);

        if (floors) {
            const floorCount = floors.length;

            for (let i = 10; i < floorCount; i++) {
                const floor = floors[i];

                if (floor.file && floor.camera) {
                    console.log(floor.modelDisplayText);
                    return floor;
                }
            }
        }

        return null;
    }

    showIndoor(modelFile/*: string*/, cameraOptions/*: Frontend.PerspectiveCameraData*/, zoneID/*: number*/) {
        if (this.prevIndoorModel && this.prevIndoorModel.name === modelFile && this.currentIndoorModel && this.currentIndoorModel.name === modelFile) {
            return;
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
                zoneID: zoneID
            };

            model.visible = true;
            this.prevIndoorModel = model;
            this.currentIndoorModel = model;
            this.currentModel = model;
            this.setMovingCamera(modelData[1], Contents3D.Mode_Indoor, param);

            this.props.setCurrentView(zoneID);
            this.useBoundingBox = false;
        }
    }

    showOutdoor(mode/*: number*/) {
        this.useBoundingBox = true;
        this.removeBoundingBox();

        if (mode === Contents3D.Mode_Indoor) {
            this.outdoorModels.map(model => {
                model.visible = false;
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

            this.outdoorModels.map(model => {
                model.visible = true;
            });

            if (this.outdoorModels.length > 0) {
                this.currentModel = this.outdoorModels[0];
            }

            // 실내 센서들 제거
            this.poiManager.removeSensors(null);
            this.poiManager.addOutdoorSensors(this.props._3dOptions.outdoorZones, this.props._3dOptions.zones, this.props.visibleSensorTypes);
        }

        if (mode !== Contents3D.Mode_Indoor) {
            this.props.setCurrentView(null);
        }
    }

    getMousePos(event/*: MouseEvent*/) {
        const x = event.nativeEvent.offsetX;
        const y = event.nativeEvent.offsetY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, this.camera);

        const intersects = raycaster.intersectObjects(this.scene.children, true);
        const intersectCount = intersects.length;

        if (intersectCount > 0) {
            const nearestIntersect = this.sortIntersects(intersects);

            if (nearestIntersect) {
                if (event.altKey && event.ctrlKey) {
                    if (this.isIndoor()) {
                        // DB에 직접 값을 저장한다.
                        this.saveIndoorModelViewport({ ...this.camera }, nearestIntersect.point);
                    }
                    else {
                        // DB에 직접 값을 저장한다.
                        this.saveOutdoorModelViewport({ ...this.camera }, nearestIntersect.point);
                    }
                }
            }
        }
    }

    traceMousePos(event/*: MouseEvent*/) {
        const x = event.nativeEvent.offsetX;
        const y = event.nativeEvent.offsetY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, this.camera);

        console.log("Mouse Position : " + raycaster.ray.origin.x + ", " + raycaster.ray.origin.z);
    }

    async saveOutdoorModelViewport(camera/*: THREE.Camera*/, targetPoint/*: THREE.Vector3*/) {
        const modelName = this.props._3dOptions.outdoorModel.file;

        if (!modelName) {
            return false;
        }

        const cameraData/*: Frontend.PerspectiveCameraData2*/ = {
            pos: {
                x: camera.position.x,
                y: camera.position.y,
                z: camera.position.z
            },
            quaternion: {
                x: camera.quaternion.x,
                y: camera.quaternion.y,
                z: camera.quaternion.z,
                w: camera.quaternion.w
            },
            rotation: {
                x: camera.rotation.x,
                y: camera.rotation.y,
                z: camera.rotation.z
            },
            orbitTarget: {
                x: targetPoint ? targetPoint.x : this.controls.target.x,
                y: targetPoint ? targetPoint.y : this.controls.target.y,
                z: targetPoint ? targetPoint.z : this.controls.target.z
            }
        };

        const result = await SDMSController.requestSaveIndoorModelViewport(modelName, cameraData);

        if (result.success) {
            let outdoorCamera = this.props._3dOptions.outdoorModel.camera;

            if (!outdoorCamera) {
                outdoorCamera = {
                    far: 5000,
                    fov: 60,
                    near: 0.1
                };

                this.props._3dOptions.outdoorModel.camera = outdoorCamera;
            }

            outdoorCamera.position = [cameraData.pos.x, cameraData.pos.y, cameraData.pos.z];
            outdoorCamera.quaternion = [cameraData.quaternion.x, cameraData.quaternion.y, cameraData.quaternion.z, cameraData.quaternion.w];
            outdoorCamera.rotation = [cameraData.rotation.x, cameraData.rotation.y, cameraData.rotation.z];
            outdoorCamera.targetControl = [cameraData.orbitTarget.x, cameraData.orbitTarget.y, cameraData.orbitTarget.z];
            return true;
        }

        return false;
    }

    async saveIndoorModelViewport(camera/*: THREE.Camera*/, targetPoint/*: THREE.Vector3 | null*/)/*: Promise<boolean>*/ {
        const modelName = this.currentModel?.name;

        if (!modelName) {
            return false;
        }

        const cameraData/*: Frontend.PerspectiveCameraData2*/ = {
            pos: {
                x: camera.position.x,
                y: camera.position.y,
                z: camera.position.z
            },
            quaternion: {
                x: camera.quaternion.x,
                y: camera.quaternion.y,
                z: camera.quaternion.z,
                w: camera.quaternion.w
            },
            rotation: {
                x: camera.rotation.x,
                y: camera.rotation.y,
                z: camera.rotation.z
            },
            orbitTarget: {
                x: targetPoint ? targetPoint.x : this.controls.target.x,
                y: targetPoint ? targetPoint.y : this.controls.target.y,
                z: targetPoint ? targetPoint.z : this.controls.target.z
            }
        };
        
        const result = await SDMSController.requestSaveIndoorModelViewport(modelName, cameraData);

        if (result?.success) {
            const index = modelName.indexOf('/');

            if (index !== null && index !== undefined) {
                const buildingGroupName = modelName.substring(0, index);
                const buildingGroup = this.props._3dOptions.indoorModels[buildingGroupName];

                if (buildingGroup) {
                    let processed = false;

                    for (const buildingName in buildingGroup) {
                        const building = buildingGroup[buildingName];

                        if (building && building.floors) {
                            const floorCount = building.floors.length;

                            for (let i = 0; i < floorCount; i++) {
                                const floor = building.floors[i];

                                if (floor.file === modelName) {
                                    if (!floor.camera) {
                                        floor.camera = {
                                            far: 5000,
                                            fov: 60,
                                            near: 0.1
                                        };
                                    }

                                    floor.camera.position = [cameraData.pos.x, cameraData.pos.y, cameraData.pos.z];
                                    floor.camera.quaternion = [cameraData.quaternion.x, cameraData.quaternion.y, cameraData.quaternion.z, cameraData.quaternion.w];
                                    floor.camera.rotation = [cameraData.rotation.x, cameraData.rotation.y, cameraData.rotation.z];
                                    floor.camera.targetControl = [cameraData.orbitTarget.x, cameraData.orbitTarget.y, cameraData.orbitTarget.z];

                                    processed = true;
                                    break;
                                }
                            }

                            if (processed) {
                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        return false;
    }

    showPipe(visible) {
        if (!this.pipeLayer) {
            return;
        }

        if (this.pipeLayer.visible && !visible) {
            this.pipeLayer.visible = false;
            this.pipeEquipmentLayer.visible = false;
            this.pipeFloorLayer.visible = false;

            const offCount = this.offObjects.length;

            for (let i = 0; i < offCount; i++) {
                const child = this.offObjects[i];
                child.visible = true;
            }

            this.offObjects = [];
            this.poiManager.showAllSensorTypes(this.props.visibleSensorTypes, { ...this.tempVisibleSensorTypes });
        }
        else if (!this.pipeLayer.visible && visible) {
            this.offObjects = [];
            const childCount = this.scene.children.length;

            for (let i = 0; i < childCount; i++) {
                const child = this.scene.children[i];

                if (child.type.includes("Light")) {
                    continue;
                }

                if (child !== this.pipeLayer &&
                    child !== this.pipeEquipmentLayer &&
                    child !== this.pipeFloorLayer) {
                    if (child.visible) {
                        child.visible = false;
                        this.offObjects.push(child);
                    }
                }
            }

            this.pipeLayer.visible = true;
            this.pipeEquipmentLayer.visible = true;
            this.tempVisibleSensorTypes = this.poiManager.hideAllSensorTypes(this.props.visibleSensorTypes);
        }
    }

    /*togglePipe() {
        if (this.pipeLayer.visible) {
            this.pipeLayer.visible = false;
            this.pipeEquipmentLayer.visible = false;
            this.pipeFloorLayer.visible = false;

            const offCount = this.offObjects.length;

            for (let i = 0; i < offCount; i++) {
                const child = this.offObjects[i];
                child.visible = true;
            }

            this.offObjects = [];
        }
        else {
            this.offObjects = [];
            const childCount = this.scene.children.length;

            for (let i = 0; i < childCount; i++) {
                const child = this.scene.children[i];

                if (child.type.includes("Light")) {
                    continue;
                }

                if (child !== this.pipeLayer &&
                    child !== this.pipeEquipmentLayer &&
                    child !== this.pipeFloorLayer) {
                    if (child.visible) {
                        child.visible = false;
                        this.offObjects.push(child);
                    }
                }
            }

            this.pipeLayer.visible = true;
            this.pipeEquipmentLayer.visible = true;
            //this.pipeFloorLayer.visible = true;
        }
    }*/

    onClick = (event/*: MouseEvent*/) => {
        if (this.state.loading) {
            return;
        }

        this.selectEquipment(event);
        this.traceMousePos(event);

        // 숨겨진 기능
        // Shift와 Ctrl Key를 누른 상태에서 Mouse Click 하면 카메라 모드를 바꾼다.
        /*if (event.ctrlKey && event.shiftKey) {
            this.togglePipe();
            //this.toggleCameraMode();
            return;
        }*/

        // 숨겨진 기능
        // Alt와 Ctrl Key를 누른 상태에서 Mouse Click 하면 현재의 Viewport를 DB에 저장한다.
        // 실내공간에서만 동작한다.
        if (event.altKey && event.ctrlKey) {
            this.getMousePos(event);
            return;
        }

        const isIndoor = this.isIndoor();
        
        if (!this.useBoundingBox) {
            return;
        }

        const poi = this.poiManager.getPOI(event, this.camera, false);
        this.onClickPOI(poi, event);

        if (this.boundingBoxModel && isIndoor === false) {
            const modelName = this.boundingBoxModel.name;

            if (event.ctrlKey) {
                const indoorModel = this.getIndoorModel(modelName);

                if (indoorModel) {
                    this.showIndoor(indoorModel.file, indoorModel.camera, indoorModel.zoneID);
                }
            }
            else {
                // 건물정보 표시
                //DataInfo.processBuildingData(modelName, this.props.showBuildingInfo);
            }
        }
    }

    selectEquipment(event) {
        if (this.prevIndoorFacility?.object?.visible) {
            const equipmentName = this.getEquipmentName(this.prevIndoorFacility.object);

            if (this.refTooltip.current) {
                this.refTooltip.current.style.top = event.nativeEvent.offsetY + "px";
                this.refTooltip.current.style.left = event.nativeEvent.offsetX + "px";
                this.refTooltip.current.style.width = "100px";

                this.visibleTooltip = true;
                this.tooltipText = equipmentName;
                this.setState({ loading: this.state.loading });
                return;
            }
        }

        this.visibleTooltip = false;
        this.tooltipText = '';
        this.setState({ loading: this.state.loading });
    }

    getEquipmentName(object) {
        if (object.name.includes(Contents3D.OutdoorFacilityHeadTag)) {
            let equipName = object.name.substring(Contents3D.OutdoorFacilityHeadTag.length).trim();

            if (equipName.startsWith('-')) {
                equipName = equipName.substring(1);
            }

            if (equipName.endsWith(SDMSDataManager.BoundingBoxTag)) {
                equipName = equipName.substring(0, equipName.length - SDMSDataManager.BoundingBoxTag.length).trim();
            }

            return equipName;
        }

        const objectName = object.name;
        const parentName = object.parent.name;

        const objLength = objectName.length;
        const parentLength = parentName.length;

        let beginIndex = -1;

        for (let i = 0; i < objLength && i < parentLength; i++) {
            if (objectName[i] !== parentName[i]) {
                beginIndex = i;
                break;
            }
        }

        if (beginIndex < 0) {
            if (objLength === parentLength) {
                beginIndex = 0;
            }
            else if (objLength > parentLength) {
                beginIndex = parentLength;
            }
            else {
                beginIndex = objLength;
            }
        }

        const equipmentName = object.name.substring(beginIndex).trim();

        if (equipmentName.startsWith('-')) {
            return equipmentName.substring(1);
        }

        return equipmentName;
    }

    onClickPOI(poi, event) {
        if (!poi) {
            this.poiManager.selectPOI(null/*, Contents3D.Edit_Mode_CCTVGroup, this.props.editModeParam*/);
            this.props.onSelectPOI(null, false, this);
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

        //this.showBuildingInfo(type, id);
        if (type === SDMSMainMenu.CCTV_Type) {
            // CCTV ID 전달하기
            this.props.onSelectCCTV(id, poi, this.poiManager);
        } else if (type === SDMSMainMenu.Worker_Type) {
            if (this.props.workers) {
                let chk = false;
                let workerInfo = null;

                for (const zoneID in this.props.workers.zones) {
                    const zoneWorkers = this.props.workers.zones[zoneID];

                    for (const workerID in zoneWorkers) {
                        const worker = zoneWorkers[workerID];

                        if (worker.id.toString() === id) {
                            workerInfo = worker;
                            chk = true;
                            break;
                        }
                    }

                    if (chk === true)
                        break;
                }

                if (workerInfo !== null) {
                    // worker 데이터 전달
                    this.props.onSelectWorker(workerInfo);
                }
            }
        }

        

        
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

        //this.clickForMovePOI(poi, event);
    }

    onMouseMove = (event/*: MouseEvent*/) => {
        if (!this.camera || !this.useBoundingBox || this.state.loading) {
            return;
        }

        this.removeBoundingBox();

        const x = event.nativeEvent.offsetX;
        const y = event.nativeEvent.offsetY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, this.camera);

        const intersects = raycaster.intersectObjects(this.scene.children, true);
        let intersectCount = intersects.length;

        for (let i = intersectCount - 1; i >= 0; i--) {
            const intersect = intersects[i];

            if (!this.isCurrentModel(intersect.object) || !intersect.object.name.includes(Contents3D.FacilityHeadTag)) {
                intersects.splice(i, 1);
                intersectCount--;
            }
        }

        if (intersectCount > 0) {
            const nearestIntersect = this.sortIntersects(intersects);
            //const nearestIntersect = this.getNearest(intersects, intersectCount);

            if (nearestIntersect) {
                if (this.pipeEquipmentLayer?.visible) {
                    const parentModel = nearestIntersect.object.parent;

                    if (parentModel === null || !parentModel.name.includes(Contents3D.FacilityHeadTag)) {
                        return;
                    }

                    const intersects = raycaster.intersectObjects(parentModel.children, true);
                    const intersectCount = intersects.length;

                    for (let i = 0; i < intersectCount; i++) {
                        const intersect = intersects[i];

                        if (intersect.object.parent === parentModel) {
                            this.setIndoorFacility(intersect);
                            return;
                        }
                    }

                    this.setIndoorFacility(null);
                }
                else if (this.currentIndoorModel === this.currentModel && this.currentModel !== null) {
                    // 실내공간
                    const internalModel = this.internalModels[this.currentModel.name];

                    if (internalModel && internalModel[4]) {
                        const parentModel = this.getParentModel(nearestIntersect.object, internalModel[4]);

                        if (!parentModel) {
                            return;
                        }

                        const intersects = raycaster.intersectObjects(parentModel.children, true);
                        const intersectCount = intersects.length;

                        for (let i = 0; i < intersectCount; i++) {
                            const intersect = intersects[i];

                            if (intersect.object.parent === parentModel) {
                                this.setIndoorFacility(intersect);
                                /*if (this.prevIndoorFacility && this.prevIndoorFacility !== intersect) {
                                    this.prevIndoorFacility.object.visible = false;
                                    this.prevIndoorFacility.object.parent.visible = false;
                                }

                                intersect.object.visible = true;
                                intersect.object.parent.visible = true;

                                this.prevIndoorFacility = intersect;*/
                                return;
                            }
                        }
                    }

                    this.setIndoorFacility(null);
                    /*if (this.prevIndoorFacility) {
                        this.prevIndoorFacility.object.visible = false;
                        this.prevIndoorFacility = null;
                    }*/
                }
                else {
                    let model = null;

                    for (let i = 0; i < intersectCount; i++) {
                        const intersect = intersects[i];

                        model = this.isBuilding(intersect.object);

                        if (model) {
                            break;
                        }
                    }

                    if (model !== null) {
                        model.visible = true;
                        this.boundingBoxModel = model;
                        return;
                    }

                    for (let i = 0; i < intersectCount; i++) {
                        const intersect = intersects[i];

                        if (intersect.object.name.startsWith(Contents3D.OutdoorFacilityHeadTag)) {
                            this.setIndoorFacility(intersect);
                            /*if (this.prevIndoorFacility && this.prevIndoorFacility !== intersect) {
                                this.prevIndoorFacility.object.visible = false;
                                this.prevIndoorFacility.object.parent.visible = false;
                            }

                            intersect.object.visible = true;
                            intersect.object.parent.visible = true;

                            this.prevIndoorFacility = intersect;*/
                            return;
                        }
                    }

                    this.setIndoorFacility(null);
                    /*if (this.prevIndoorFacility) {
                        this.prevIndoorFacility.object.visible = false;
                        this.prevIndoorFacility = null;
                    }*/
                }
            }
        }
        else {
            this.setIndoorFacility(null);
            /*if (this.prevIndoorFacility) {
                this.prevIndoorFacility.object.visible = false;
                this.prevIndoorFacility = null;
            }*/
        }
    }

    setIndoorFacility(facility) {
        if (facility === null) {
            if (this.prevIndoorFacility) {
                this.prevIndoorFacility.object.visible = false;

                if (this.prevIndoorFacility.object.name.includes(Contents3D.OutdoorFacilityHeadTag) === false) {
                    this.prevIndoorFacility.object.parent.visible = false;
                }

                this.prevIndoorFacility = null;
            }
        }
        else {
            if (this.prevIndoorFacility && this.prevIndoorFacility !== facility) {
                this.prevIndoorFacility.object.visible = false;

                if (this.prevIndoorFacility.object.name.includes(Contents3D.OutdoorFacilityHeadTag) === false) {
                    this.prevIndoorFacility.object.parent.visible = false;
                }
            }

            this.prevIndoorFacility = facility;
            this.prevIndoorFacility.object.visible = true;

            if (this.prevIndoorFacility.object.name.includes(Contents3D.OutdoorFacilityHeadTag) === false) {
                this.prevIndoorFacility.object.parent.visible = true;
            }
        }
    }

    getParentModel(obj, model) {
        const childCount = model.children.length;

        for (let i = 0; i < childCount; i++) {
            if (model.children[i] === obj) {
                return model;
            }
        }

        const parent = model.parent;

        if (!parent) {
            return null;
        }

        const modelCount = parent.children.length;

        for (let i = 0; i < modelCount; i++) {
            const _model = parent.children[i];

            if (_model === model) {
                continue;
            }

            const count = _model.children.length;

            for (let j = 0; j < count; j++) {
                if (_model.children[j] === obj) {
                    return _model;
                }
            }
        }

        return null;
    }

    isCurrentModel(obj) {
        if (this.pipeEquipmentLayer?.visible) {
            return true;
        }
        else {
            const currentModelName = this.currentModel ? this.currentModel.name : null;

            if (!currentModelName) {
                return true;
            }

            while (obj) {
                if (obj.name === currentModelName) {
                    return true;
                }

                obj = obj.parent;
            }
        }

        return false;
    }

    getAlarmElements() {
        if (!this.props.alarmSound) {
            return (
                <></>
            );
        }

        if (this.state.alarm === Contents3D.ALARM_2) {
            return (
                <audio autoPlay={true} loop={true}
                    src="/resource/sound/alarm_level2.mp3">
                </audio>
            );
        }
        else if (this.state.alarm === Contents3D.ALARM_3) {
            return (
                <audio autoPlay={true} loop={true}
                    src="/resource/sound/alarm_level3.mp3">
                </audio>
            );
        }
        else if (this.state.alarm === Contents3D.ALARM_4) {
            return (
                <audio autoPlay={true} loop={true}
                    src="/resource/sound/alarm_level4.mp3">
                </audio>
            );
        }

        return (
            <></>
        );
    }

    // BoundingBox Check
    isBuilding(obj/*: THREE.Object3D*/)/*: THREE.Object3D | null*/ {
        if (obj.name.endsWith(SDMSDataManager.BoundingBoxTag) === false) {
            if (obj.parent === null) {
                return null;
            }

            return this.isBuilding(obj.parent);
        }

        const len = obj.name.length;
        const objName = obj.name.substring(0, len - SDMSDataManager.BoundingBoxTag.length);

        const building = this.props._3dOptions.allBuildings[objName];

        if (building) {
            return obj;
        }

        if (obj.parent === null) {
            return null;
        }

        return this.isBuilding(obj.parent);
    }

    // objects를 가까운 순서대로 정렬한다.
    sortIntersects(objects) {
        objects.sort((obj1, obj2) => {
            if (obj1.distance < obj2.distance) {
                return -1;
            }
            else if (obj1.distance > obj2.distance) {
                return 1;
            }

            return 0;
        });

        return objects[0];
    }

    setMovingCamera(cameraOptions/*: Frontend.PerspectiveCameraData*/, mode/*: number*/, param/*: any*/) {
        this.setState({ loading: true });

        if (cameraOptions === null || cameraOptions === undefined) {
            return;
        }

    const distancePos = Geometry.getDistance3(this.camera.position.x, this.camera.position.y, this.camera.position.z, cameraOptions.position[0]/* as number*/, cameraOptions.position[1]/* as number*/, cameraOptions.position[2]/* as number*/);
    const distanceQua = cameraOptions.quaternion === null ? null : Geometry.getDistance4(this.camera.quaternion.x, this.camera.quaternion.y, this.camera.quaternion.z, this.camera.quaternion.w, cameraOptions.quaternion[0]/* as number*/, cameraOptions.quaternion[1]/* as number*/, cameraOptions.quaternion[2]/* as number*/, cameraOptions.quaternion[3]/* as number*/);
    const distanceRot = Geometry.getDistance3(this.camera.rotation.x, this.camera.rotation.y, this.camera.rotation.z, cameraOptions.rotation[0]/* as number*/, cameraOptions.rotation[1]/* as number*/, cameraOptions.rotation[2]/* as number*/);

        this.movingCamera = {
            // 초
            movingTime: 0.75,
            //movingTime: 1.5,
            elapsedTime: 0,
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

    onChangeTooltipText() {
    }

    _showPipe(visible) {
        this.showPipe(visible);
        this.setState({ loading: this.state.loading });
    }

    getViewModeButtons() {
        if (ProjectResource.isModelViewer) {
            if (!this.pipeLayer || !this.pipeLayer.visible) {
                return (
                    <div className={styles.cameraButtonArea}>
                        <span className={styles.floorBtnAct} onClick={() => this._showPipe(false)}>층별</span>
                        <span className={styles.entireBtnDis} onClick={() => this._showPipe(true)}>전체</span>
                    </div>
                );
            }
            else {
                return (
                    <div className={styles.cameraButtonArea}>
                        <span className={styles.floorBtnDis} onClick={() => this._showPipe(false)}>층별</span>
                        <span className={styles.entireBtnAct} onClick={() => this._showPipe(true)}>전체</span>
                    </div>
                );
            }
        }

        return <></>;
    }
    
    render() {
        const className = this.state.loading ? styles.contents3DArea + " " + styles.loading : styles.contents3DArea;
        const tooltipClassName = this.visibleTooltip ? styles.tooltip : styles.tooltip + " " + styles.hide;

        this.poiManager.setVisibleSensorTypes(this.props.visibleSensorTypes);

        return (
            <main className={styles.appWrap}>
                <section className={styles.appContainerWrap}>
                    <div ref={this.ref3D} className={className} onClick={this.onClick} onMouseMove={this.onMouseMove}>

                    {
                        this.getViewModeButtons()
                    }
                    </div>
                </section>
                <figure>
                    {
                        this.getAlarmElements()
                    }
                </figure>
                <input ref={this.refTooltip} type="text" className={tooltipClassName} value={this.tooltipText} onChange={() => this.onChangeTooltipText()} />
            </main>
        );
    }
}

export default Contents3D;