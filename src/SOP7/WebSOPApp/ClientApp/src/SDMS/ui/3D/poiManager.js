import * as THREE from "three/build/three.module.js";
import Geometry from "../../../Common/util/Geometry";
import ProjectResource from "../../../Root/resource/id";
import { SDMSController } from "../../services/sdmsController";
import { SDMSDataManager } from "../../services/sdmsDataManager";
import CCTVInfo from "../popups/cctvInfo";
import SDMS from "../sdms";
import SDMSMainMenu from "../sdmsMainMenu";
import Contents3D from "./contents3D";
import { TextPOIManager } from "./textPOIManager";
import SettingsStore from '../../../Settings/settingsStore';
import SettingResource from '../../../Settings/resource/id';

export class POIManager {
    static OutdoorPoiScale = 4;
    static IndoorPoiScale = 1;

    static MinusHeight = 20;

    static Safety_I_Type = "Safety-I";
    static PTZ_Type = "PTZ";

    static MovingForwardTime = 0.75;
    static MovingBackwardTime = 0.5;

    static SelectedPoiScale = 2;
    static EquipZonePoiScale = 1.5;
    static OriginPoiScale = 1;

    scene = null;
    spriteMaterials = { /*[url: string]: THREE.SpriteMaterial*/ };
    sensorPOIs = {};
    selectedPOI = null;
    sensorLayers = {};
    // 새로운 CCTV 추가를 위한 임시 POI
    tempOutdoorCCTVPOI = null;

    smoothScalePois = [];

    constructor(contents3D) {
        this.contents3D = contents3D;
    }

    get Scene() {
        return this.scene;
    }

    set Scene(scene) {
        this.scene = scene;
    }

    getSensorPOI(sensorType, zoneID, sensorID) {
        const key = this.getSensorKey(sensorType, zoneID, sensorID);
        const sprite = this.sensorPOIs[key];
        return sprite;
    }

    getSensorKey(sensorType, zoneID, sensorID) {
        return sensorType + "_" + zoneID + "_" + sensorID;
    }

    getSensorType(sensorName)/*: string | null*/ {
        // 센서 타입 가져오기
        // Name 규칙이 타입 + "_" + Zone ID + "_" SensorID 인 것을 이용
        const index = sensorName.indexOf("_");
        if (index < 0)
            return null;

        const type = sensorName.substring(0, index);
        return type;
    }

    getSensorID(sensorName)/*: string | null*/ {
        // 해당 센서 아이디 가져오기
        // Name 규칙이 타입 + "_" + Zone ID + "_" SensorID 인 것을 이용

        let index = sensorName.indexOf("_");
        if (index < 0)
            return null;

        //const type = sensorName.substring(0, index);
        const zone_id = sensorName.substring(index + 1);

        index = zone_id.indexOf("_");
        if (index < 0)
            return null;

        //const zone = zone_id.substring(0, index);
        const id = zone_id.substring(index + 1);

        return id;
    }

    getSensorZoneID(id, sensorType) {
        if (this.sensorPOIs) {
            for (const sensorName in this.sensorPOIs) {
                // 센서 타입 가져오기
                // Name 규칙이 타입 + "_" + Zone ID + "_" SensorID 인 것을 이용
                const index1 = sensorName.indexOf("_");
                if (index1 < 0)
                    continue;

                const index2 = sensorName.lastIndexOf("_");
                if (index2 < 0)
                    continue;

                const zoneID = sensorName.substring(index1 + 1, index2).trim();
                return parseInt(zoneID);
            }
        }

        return NaN;
    }

    getSensorInfo(sensorName) {
        const index1 = sensorName.lastIndexOf('_');

        if (index1 < 0)
            return null;

        const sensorID = parseInt(sensorName.substring(index1 + 1).trim());
        const sensorData = sensorName.substring(0, index1);

        const index2 = sensorData.lastIndexOf('_');

        if (index2 < 0)
            return null;

        const zoneID = parseInt(sensorData.substring(index2 + 1).trim());
        const sensorType = sensorData.substring(0, index2).trim();

        return [sensorType, zoneID, sensorID];
    }

    removeSensors(sensorType) {
        const removeKeys = [];
        const removeNames = {};

        if (sensorType) {
            for (const sensorID in this.sensorPOIs) {
                if (sensorID.startsWith(sensorType)) {
                    //const sprite = this.sensorPOIs[sensorID];
                    //this.scene.remove(sprite);
                    removeKeys.push(sensorID);

                    removeNames[sensorID] = sensorID;
                }
            }

            this.clearSensorType(sensorType);
        }
        else {
            for (const sensorID in this.sensorPOIs) {
                //const sprite = this.sensorPOIs[sensorID];
                //this.scene.remove(sprite);
                removeKeys.push(sensorID);

                removeNames[sensorID] = sensorID;
            }

            for (const sensorType in this.sensorLayers) {
                this.clearSensorType(sensorType);
            }
        }

        const sceneChildCount = this.scene.children.length;

        for (let i = sceneChildCount - 1; i >= 0; i--) {
            const child = this.scene.children[i];

            if (removeNames[child.name]) {
                this.scene.remove(child);
            }
        }

        const removeCount = removeKeys.length;

        for (let i = 0; i < removeCount; i++) {
            const sensorID = removeKeys[i];
            delete this.sensorPOIs[sensorID];
        }
    }

    clearSensorType(sensorType) {
        const sensors = this.sensorLayers[sensorType];

        if (sensors) {
            const sensorCount = sensors.children.length;

            for (let i = sensorCount - 1; i >= 0; i--) {
                const sensor = sensors.children[i];
                sensors.remove(sensor);
            }
        }
    }

    remove(sensorType, poi) {
        const sensors = this.sensorLayers[sensorType];

        if (sensors) {
            sensors.remove(poi);
        }
    }

    moveSensor(sensorType, sensorID, zoneID, x, y, z) {
        const key = sensorType + "_" + zoneID + "_" + sensorID;
        const sprite = this.sensorPOIs[key];

        if (sprite) {
            sprite.position.x = x;
            sprite.position.y = y;
            sprite.position.z = z;
        }
    }

    selectPOI(poi, editMode, editModeParam) {
        if (editMode === Contents3D.Edit_Mode_CCTVGroup &&
            (editModeParam === CCTVInfo.Mode_Select_CCTV || editModeParam === CCTVInfo.Mode_Delete_CCTV)) {
            const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(poi);

            if (sensorType === SDMSMainMenu.CCTV_Type ||
                sensorType === SDMSMainMenu.CCTV_PTZ_Type) {
                return;
            }
        }

        if (!poi) {
            if (!this.selectedPOI) {
                return;
            }
            else {
                /*if (editMode !== Contents3D.Edit_Mode_CCTVGroup)*/ {
                    this.changePOI(this.selectedPOI, false);
                }
            }
        }
        else {
            if (POIManager.isSamePOI(this.selectedPOI, poi) === false) {
                if (this.selectedPOI !== null) {
                    this.changePOI(this.selectedPOI, false);
                }

                /*if (editMode !== Contents3D.Edit_Mode_CCTVGroup)*/ {
                    this.changePOI(poi, true);
                }
            }
        }

        this.selectedPOI = poi;
    }

    static isSamePOI(poi1, poi2) {
        if (poi1 === null && poi2 === null) {
            return true;
        }
        else if (poi1 === null) {
            return false;
        }
        else if (poi2 === null) {
            return false;
        }

        const obj1 = poi1.object ? poi1.object : poi1;
        const obj2 = poi2.object ? poi2.object : poi2;
        return obj1.uuid === obj2.uuid;
    }

    changePOI(poi, isSelected) {
        if (poi) {
            const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(poi);

            if (sensorType) {
                let sensor = SDMSDataManager.getSensor(sensorType, zoneID, sensorID, this.contents3D.props._3dOptions);

                if (!sensor && sensorType === SDMSMainMenu.CCTV_Type) {
                    sensor = this.getNewCCTV(sensorID);
                }

                if (sensor) {
                    const enabled = sensor.enabled === null || sensor.enabled === true;
                    this.changePoiObject(poi.object, isSelected, enabled);
                }
            }

            this.changeEquipZonePois(sensorType, sensorID, zoneID, isSelected);
        }
    }

    // isSelect가 true일 경우 같은 EquipZone 내의 Sensor들은 활성화(다른 아이콘들과 다르게 보이도록) 시켜준다.
    async changeEquipZonePois(sensorType, sensorID, zoneID, isSelected) {
        const result = await SDMSController.requestEquipZoneSensorList(sensorType, sensorID);

        if (result && result.success) {
            const sensorCount = result.sensorIDs.length;

            for (let i = 0; i < sensorCount; i++) {
                const id = result.sensorIDs[i];

                if (id === sensorID) {
                    const _poi = this.getSensorPOI(sensorType, zoneID, id);

                    if (_poi === null || _poi === undefined)
                        continue;

                    if (isSelected) {
                        _poi.position.y = _poi.userData.origin.position.y + 1;
                    }
                    else {
                        _poi.position.y = _poi.userData.origin.position.y;
                    }

                    continue;
                }

                const poi = this.getSensorPOI(sensorType, zoneID, id);

                if (poi) {
                    if (isSelected) {
                        this.setPoiTargetScale(poi, POIManager.EquipZonePoiScale, POIManager.MovingForwardTime);
                    }
                    else if (POIManager.isSamePOI(poi, this.selectedPOI) === false) {
                        this.setPoiTargetScale(poi, POIManager.OriginPoiScale, POIManager.MovingBackwardTime);
                        poi.position.y = poi.userData.origin.position.y;
                    }
                }
            }
        }
    }

    getNewCCTV(sensorID) {
        const newCCTVList = this.contents3D?.props?.newCCTVList;

        if (newCCTVList) {
            const count = newCCTVList.length;

            for (let i = 0; i < count; i++) {
                const cctv = newCCTVList[i];

                if (cctv.id === sensorID) {
                    return cctv;
                }
            }
        }

        return null;
    }

    setPoiTargetScale(obj, targetScale, movingTime) {
        let usePoiHighlight = this.contents3D.state.commonSettings?.UsePoiHighlight;

        //if (ProjectResource.siteID === ProjectResource.Site.Soulbrain) {
        if (usePoiHighlight !== SettingResource.usePoiHighlight.on) {
            // Soulbrain에서는 아이콘 크기변경 사용하지 않음
            return;
        }

        obj.userData.targetScale = {
            tx: obj.userData.origin.scale.x * targetScale,
            ty: obj.userData.origin.scale.y * targetScale,
            tz: obj.userData.origin.scale.z * targetScale,
            bx: obj.scale.x,
            by: obj.scale.y,
            bz: obj.scale.z,
            movingTime: movingTime,
            elapsedTime: 0
        };

        const index = this.smoothScalePois.indexOf(obj);

        if (index < 0) {
            this.smoothScalePois.push(obj);
        }
    }

    changePoiScale(obj, delta, index) {
        obj.userData.targetScale.elapsedTime += delta;

        const elapsedTime = obj.userData.targetScale.elapsedTime;
        const movingTime = obj.userData.targetScale.movingTime;

        if (elapsedTime >= movingTime) {
            obj.scale.x = obj.userData.targetScale.tx;
            obj.scale.y = obj.userData.targetScale.ty;
            obj.scale.z = obj.userData.targetScale.tz;

            this.smoothScalePois.splice(index, 1);
        }
        else {
            obj.scale.x = obj.userData.targetScale.bx + (obj.userData.targetScale.tx - obj.userData.targetScale.bx) * elapsedTime / movingTime;
            obj.scale.y = obj.userData.targetScale.by + (obj.userData.targetScale.ty - obj.userData.targetScale.by) * elapsedTime / movingTime;
            obj.scale.z = obj.userData.targetScale.bz + (obj.userData.targetScale.tz - obj.userData.targetScale.bz) * elapsedTime / movingTime;
        }
    }

    changePoiScales(delta) {
        const count = this.smoothScalePois.length;

        for (let i = count - 1; i >= 0; i--) {
            const poi = this.smoothScalePois[i];
            this.changePoiScale(poi, delta, i);
        }
    }

    changePoiObject(obj, isSelected, enabled) {
        let imgURL = null;

        if (obj) {
            if (isSelected) {
                if (enabled) {
                    imgURL = obj.material.userData["selected"];
                }
                else {
                    imgURL = obj.material.userData["disabledSelected"];
                }

                this.setPoiTargetScale(obj, POIManager.SelectedPoiScale, POIManager.MovingForwardTime);
            }
            else {
                if (enabled) {
                    imgURL = obj.material.userData["origin"];
                }
                else {
                    imgURL = obj.material.userData["disabled"];
                }

                this.setPoiTargetScale(obj, POIManager.OriginPoiScale, POIManager.MovingBackwardTime);
            }

            if (imgURL && imgURL.length > 0) {
                let spriteMaterial = this.spriteMaterials[imgURL];

                if (!spriteMaterial) {
                    const spriteMap = new THREE.TextureLoader().load(imgURL);
                    spriteMaterial = new THREE.SpriteMaterial({ map: spriteMap, color: 0xffffff });
                    this.spriteMaterials[imgURL] = spriteMaterial;

                    spriteMaterial.userData["origin"] = this.getOriginImageURL(imgURL);
                    spriteMaterial.userData["selected"] = this.getSelectedImageURL(imgURL);
                    spriteMaterial.userData["disabled"] = this.getDisabledImageURL(imgURL);
                    spriteMaterial.userData["disabledSelected"] = this.getDisabledSelectedImageURL(imgURL);

                    /*if (isSelected) {
                        spriteMaterial.userData["origin"] = this.getOriginImageURL(imgURL);
                        spriteMaterial.userData["selected"] = imgURL;
                    }
                    else {
                        spriteMaterial.userData["origin"] = imgURL;
                        spriteMaterial.userData["selected"] = this.getSelectedImageURL(imgURL);
                    }*/
                }

                obj.material = spriteMaterial;
            }
        }
    }

    selectEquipZoneCCTVs(equipZoneCCTV) {
        if (this.selectedPOI) {
            const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(this.selectedPOI);

            if (sensorType === SDMSMainMenu.CCTV_Type ||
                sensorType === SDMSMainMenu.CCTV_PTZ_Type) {
                this.selectedPOI = null;
            }
        }

        this._selectEquipZoneCCTVs(SDMSMainMenu.CCTV_Type, equipZoneCCTV);
        this._selectEquipZoneCCTVs(SDMSMainMenu.CCTV_SafetyI_Type, equipZoneCCTV);
        this._selectEquipZoneCCTVs(SDMSMainMenu.CCTV_PTZ_Type, equipZoneCCTV);
    }

    _selectEquipZoneCCTVs(layerType, equipZoneCCTV) {
        const cctvLayer = this.sensorLayers[layerType];

        if (cctvLayer) {
            const cctvCount = cctvLayer.children.length;

            for (let i = 0; i < cctvCount; i++) {
                const cctv = cctvLayer.children[i];
                const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(cctv);

                if (sensorType) {
                    const sensor = SDMSDataManager.getSensor(sensorType, zoneID, sensorID, this.contents3D.props._3dOptions);

                    if (sensor) {
                        const enabled = sensor.enabled === null || sensor.enabled === true;

                        if (this.isEqiupZoneCCTV(equipZoneCCTV, sensorID)) {
                            this.changePoiObject(cctv, true, enabled);
                        }
                        else {
                            this.changePoiObject(cctv, false, enabled);
                        }
                    }
                }
            }
        }
    }

    isEqiupZoneCCTV(equipZoneCCTV, id) {
        if (equipZoneCCTV) {
            if (equipZoneCCTV.cctV1 === id)
                return true;
            else if (equipZoneCCTV.cctV2 === id)
                return true;
            else if (equipZoneCCTV.cctV3 === id)
                return true;
            else if (equipZoneCCTV.cctV4 === id)
                return true;
        }

        return false;
    }

    getOriginImageURL(imgURL) {
        const dotIndex = imgURL.lastIndexOf('.');
        const index = imgURL.indexOf('_');

        let ext = "";

        if (dotIndex > 0) {
            ext = imgURL.substring(dotIndex).trim();
        }

        if (index > 0) {
            const fileName = imgURL.substring(0, index);
            return fileName + ext;
        }

        return imgURL;
        /*const target = "_selected";
        const index = imgURL.lastIndexOf(target);

        if (index < 0) {
            return imgURL;
        }

        const left = imgURL.substring(0, index);
        const right = imgURL.substring(index + target.length);
        return left + right;*/
    }

    getSelectedImageURL(imgURL) {
        const target = "_selected";
        const dotIndex = imgURL.lastIndexOf('.');
        const index = imgURL.indexOf('_');

        let ext = "";

        if (dotIndex > 0) {
            ext = imgURL.substring(dotIndex).trim();
        }

        if (index > 0) {
            const fileName = imgURL.substring(0, index);
            return fileName + target + ext;
        }
        else if (dotIndex > 0) {
            const fileName = imgURL.substring(0, dotIndex);
            return fileName + target + ext;
        }

        return imgURL;
        /*const target = "_selected";
        const index = imgURL.lastIndexOf('.');

        if (index < 0) {
            return imgURL + target;
        }

        const left = imgURL.substring(0, index) + target;
        const right = imgURL.substring(index);
        return left + right;*/
    }

    getDisabledImageURL(imgURL) {
        const target = "_disabled";
        const dotIndex = imgURL.lastIndexOf('.');
        const index = imgURL.indexOf('_');

        let ext = "";

        if (dotIndex > 0) {
            ext = imgURL.substring(dotIndex).trim();
        }

        if (index > 0) {
            const fileName = imgURL.substring(0, index);
            return fileName + target + ext;
        }
        else if (dotIndex > 0) {
            const fileName = imgURL.substring(0, dotIndex);
            return fileName + target + ext;
        }

        return imgURL;
        /*const target = "_disabled";
        const index = imgURL.lastIndexOf('.');

        if (index < 0) {
            return imgURL + target;
        }

        const left = imgURL.substring(0, index) + target;
        const right = imgURL.substring(index);
        return left + right;*/
    }

    getDisabledSelectedImageURL(imgURL) {
        const target = "_disabled_selected";
        const dotIndex = imgURL.lastIndexOf('.');
        const index = imgURL.indexOf('_');

        let ext = "";

        if (dotIndex > 0) {
            ext = imgURL.substring(dotIndex).trim();
        }

        if (index > 0) {
            const fileName = imgURL.substring(0, index);
            return fileName + target + ext;
        }
        else if (dotIndex > 0) {
            const fileName = imgURL.substring(0, dotIndex);
            return fileName + target + ext;
        }

        return imgURL;
        /*const target = "_disabled_selected";
        const index = imgURL.lastIndexOf('.');

        if (index < 0) {
            return imgURL + target;
        }

        const left = imgURL.substring(0, index) + target;
        const right = imgURL.substring(index);
        return left + right;*/
    }

    addTempCCTVPOI() {
        const sensorType = SDMSMainMenu.CCTV_Type;
        const url = this.getSensorImageURL(sensorType, null);
        const spriteOutdoor = this.addPOI(url, 0, 40, 0, POIManager.OutdoorPoiScale, sensorType);
        const spriteIndoor = this.addPOI(url, 0, 40, 0, POIManager.IndoorPoiScale, sensorType);

        // Temp POI이기 때문에 SensorLayer에 넣지 않고 Scene에 직접 넣는다.
        this.getSensorLayer(sensorType).remove(spriteOutdoor);
        this.getSensorLayer(sensorType).remove(spriteIndoor);
        this.scene.add(spriteOutdoor);
        this.scene.add(spriteIndoor);

        spriteOutdoor.visible = false;
        spriteIndoor.visible = false;
        this.tempOutdoorCCTVPOI = spriteOutdoor;
        this.tempIndoorCCTVPOI = spriteIndoor;
    }

    registSelectedImage(imgURL) {
        const selectedImageURL = this.getSelectedImageURL(imgURL);

        const spriteMapSelected = new THREE.TextureLoader().load(selectedImageURL);
        const spriteMaterialSelected = new THREE.SpriteMaterial({ map: spriteMapSelected, color: 0xffffff });
        this.spriteMaterials[selectedImageURL] = spriteMaterialSelected;

        return [selectedImageURL, spriteMaterialSelected];
    }

    registDisabledImage(imgURL) {
        const disabledImageURL = this.getDisabledImageURL(imgURL);

        const spriteMapDisabled = new THREE.TextureLoader().load(disabledImageURL);
        const spriteMaterialDisabled = new THREE.SpriteMaterial({ map: spriteMapDisabled, color: 0xffffff });
        this.spriteMaterials[disabledImageURL] = spriteMaterialDisabled;

        return [disabledImageURL, spriteMaterialDisabled];
    }

    registDisabledSelectedImage(imgURL) {
        const disabledSelectedImageURL = this.getDisabledSelectedImageURL(imgURL);

        const spriteMapDisabledSelected = new THREE.TextureLoader().load(disabledSelectedImageURL);
        const spriteMaterialDisabledSelected = new THREE.SpriteMaterial({ map: spriteMapDisabledSelected, color: 0xffffff });
        this.spriteMaterials[disabledSelectedImageURL] = spriteMaterialDisabledSelected;

        return [disabledSelectedImageURL, spriteMaterialDisabledSelected];
    }

    setMaterials(spriteMaterial, spriteMaterialSelected, spriteMaterialDisabled, spriteMaterialDisabledSelected, imgURL, selectedImageURL, disabledImageURL, disabledSelectedImageURL) {
        spriteMaterial.userData["origin"] = imgURL;
        spriteMaterial.userData["selected"] = selectedImageURL;
        spriteMaterial.userData["disabled"] = disabledImageURL;
        spriteMaterial.userData["disabledSelected"] = disabledSelectedImageURL;

        spriteMaterialSelected.userData["origin"] = imgURL;
        spriteMaterialSelected.userData["selected"] = selectedImageURL;
        spriteMaterialSelected.userData["disabled"] = disabledImageURL;
        spriteMaterialSelected.userData["disabledSelected"] = disabledSelectedImageURL;

        spriteMaterialDisabled.userData["origin"] = imgURL;
        spriteMaterialDisabled.userData["selected"] = selectedImageURL;
        spriteMaterialDisabled.userData["disabled"] = disabledImageURL;
        spriteMaterialDisabled.userData["disabledSelected"] = disabledSelectedImageURL;

        spriteMaterialDisabledSelected.userData["origin"] = imgURL;
        spriteMaterialDisabledSelected.userData["selected"] = selectedImageURL;
        spriteMaterialDisabledSelected.userData["disabled"] = disabledImageURL;
        spriteMaterialDisabledSelected.userData["disabledSelected"] = disabledSelectedImageURL;
    }

    addPOI(imgURL, x, y, z, scale, sensorType) {
        let spriteMaterial = this.spriteMaterials[imgURL];

        if (!spriteMaterial) {
            const spriteMap = new THREE.TextureLoader().load(imgURL);
            spriteMaterial = new THREE.SpriteMaterial({ map: spriteMap, color: 0xffffff });
            this.spriteMaterials[imgURL] = spriteMaterial;

            const [selectedImageURL, spriteMaterialSelected] = this.registSelectedImage(imgURL);
            const [disabledImageURL, spriteMaterialDisabled] = this.registDisabledImage(imgURL);
            const [disabledSelectedImageURL, spriteMaterialDisabledSelected] = this.registDisabledSelectedImage(imgURL);
            /*const selectedImageURL = this.getSelectedImageURL(imgURL);

            const spriteMapSelected = new THREE.TextureLoader().load(selectedImageURL);
            const spriteMaterialSelected = new THREE.SpriteMaterial({ map: spriteMapSelected, color: 0xffffff });
            this.spriteMaterials[selectedImageURL] = spriteMaterialSelected;*/

            this.setMaterials(spriteMaterial, spriteMaterialSelected, spriteMaterialDisabled, spriteMaterialDisabledSelected, imgURL, selectedImageURL, disabledImageURL, disabledSelectedImageURL);
            /*spriteMaterial.userData["origin"] = imgURL;
            spriteMaterial.userData["selected"] = selectedImageURL;

            spriteMaterialSelected.userData["origin"] = imgURL;
            spriteMaterialSelected.userData["selected"] = selectedImageURL;*/
        }

        const sprite = new THREE.Sprite(spriteMaterial);

        //sprite.material.depthWrite = false;
        //sprite.material.depthTest = false;

        sprite.scale.x *= 1.4 * scale;
        sprite.scale.y *= 1.75 * scale;
        sprite.scale.z *= 1.75 * scale;

        sprite.position.x = x;
        sprite.position.y = y;
        sprite.position.z = z;

        sprite.userData.origin = {
            scale: {
                x: sprite.scale.x,
                y: sprite.scale.y,
                z: sprite.scale.z
            },
            position: {
                x: x,
                y: y,
                z: z
            }
        };

        this.getSensorLayer(sensorType).add(sprite);
        //this.scene.add(sprite);

        return sprite;
    }

    removePOI(sensorType, zoneID, sensorID) {
        const key = this.getSensorKey(sensorType, zoneID, sensorID);
        const sprite = this.sensorPOIs[key];

        if (sprite) {
            const sensors = this.sensorLayers[sensorType];

            if (sensors) {
                sensors.remove(sprite);
            }

            //this.scene.remove(sprite);
            delete this.sensorPOIs[key];
            return true;
        }

        return false;
    }

    movePOI(sensorType, zoneID, sensorID, x, y, z) {
        const sprite = this.getSensorPOI(sensorType, zoneID, sensorID);

        if (sprite) {
            sprite.object.position.x = x;
            sprite.object.position.y = y;
            sprite.object.position.z = z;
        }
    }

    getPOI(event, camera, isEditTextMode) {
        const x = event.nativeEvent.offsetX;
        const y = event.nativeEvent.offsetY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, camera);

        const intersects = raycaster.intersectObjects(this.scene.children, true);
        const intersectCount = intersects.length;

        for (let i = 0; i < intersectCount; i++) {
            const intersect = intersects[i];

            if (intersect.object.visible === false || (intersect.object.parent && intersect.object.parent.visible === false)) {
                continue;
            }

            if (POIManager.isSprite(intersect) && intersect.object.name.length > 0) {
                const sensorType = this.getSensorType(intersect.object.name);

                if (!isEditTextMode && TextPOIManager.isTextPOI(sensorType) === false) {
                    return intersect;
                }
                else {
                    if (isEditTextMode && TextPOIManager.hitTest(intersect, raycaster.ray.origin.x, raycaster.ray.origin.z)) {
                        return intersect;
                    }
                }
            }
        }

        return null;
    }

    getBuildingTextPOI(event, camera) {
        const raycaster = this.getRay(event, camera, 0, 0);
        
        const intersects = raycaster.intersectObjects(this.scene.children, true);
        const intersectCount = intersects.length;

        for (let i = 0; i < intersectCount; i++) {
            const intersect = intersects[i];

            if (intersect.object.visible === false || (intersect.object.parent && intersect.object.parent.visible === false)) {
                continue;
            }

            if (POIManager.isSprite(intersect) && intersect.object.name.length > 0) {
                const sensorType = this.getSensorType(intersect.object.name);

                if (sensorType === SDMSMainMenu.BuildingGroupNameText || sensorType === SDMSMainMenu.BuildingNameText) {
                    if (TextPOIManager.isTextPOI(sensorType)) {
                        
                        const uvArea = intersect.object.userData?.uvArea;

                        if (uvArea) {
                            if (intersect.uv.x <= uvArea.right && intersect.uv.x >= uvArea.left && intersect.uv.y <= uvArea.top && intersect.uv.y >= uvArea.bottom) {
                                const [_sensorType, zoneID, sensorID] = SDMS.getSensorInfo(intersect);
                                return [sensorID, sensorType];
                            }
                        }

                        //if (TextPOIManager.hitTest3D(intersect, raycaster, leftRay, topRay/*, outResult*/)) {
                        //    const [_sensorType, zoneID, sensorID] = SDMS.getSensorInfo(intersect);
                        //    return [sensorID, sensorType];
                        //}
                        /*else {
                            const distance = outResult.distanceFromCenter;

                            if (distance !== undefined && distance !== null) {
                                if (nearestDistance === null || nearestDistance > distance) {
                                    nearestDistance = distance;
                                    nearestSensor = [sensorType, intersect];
                                }
                            }
                        }*/
                    }
                }
            }
        }

        return [null, null];
    }

    getRay(event, camera, moveX, moveY) {
        const x = event.nativeEvent.offsetX + moveX;
        const y = event.nativeEvent.offsetY + moveY;
        const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

        const raycaster = new THREE.Raycaster();
        raycaster.setFromCamera(mouse, camera);
        return raycaster;
    }

    addOutdoorSensors(outdoorZones, zones, visibleSensorTypes) {
        for (const zoneID in outdoorZones) {
            this.addZoneSensors(parseInt(zoneID), POIManager.OutdoorPoiScale, outdoorZones, zones, visibleSensorTypes);
        }

        this.addTempCCTVPOI();
    }

    addZoneSensors(zoneID, scale, outdoorZones, zones, visibleSensorTypes) {
        let zone = zones[zoneID.toString()];

        if (!zone) {
            zone = outdoorZones[zoneID.toString()];
        }

        if (zone) {
            for (const sensorType in visibleSensorTypes) {
                const visible = visibleSensorTypes[sensorType];

                //if (visible) {
                const sensors = zone.sensors[sensorType];

                if (sensors) {
                    // 편집 내용이 있다면 적용 - K.D.R
                    this.applyToEditSensors(sensors, sensorType, zoneID);

                    this.addSensors(sensorType, sensors, scale, zoneID);
                }
                //}
            }
            /*const sensorTypeCount = visibleSensorTypes.length;

            for (let i = 0; i < sensorTypeCount; i++) {
                const sensorType = visibleSensorTypes[i];
                const sensors = zone.sensors[sensorType];

                if (sensors) {
                    this.addSensors(sensorType, sensors, scale, zoneID);
                }
            }*/
        }
    }

    addSensor(sensorType, sensorID, x, y, z, zoneID, isIndoor) {
        if (zoneID === null || zoneID === undefined) {
            return null;
        }

        if (x === null || x === undefined ||
            y === null || y === undefined ||
            z === null || z === undefined) {
            return null;
        }

        const scale = isIndoor ? POIManager.IndoorPoiScale : POIManager.OutdoorPoiScale;

        if (sensorType === SDMSMainMenu.CCTV_Type) {
            const cctv = this.getCCTV(sensorID, zoneID);

            if (cctv) {
                if (cctv.type === POIManager.Safety_I_Type) {
                    sensorType = SDMSMainMenu.CCTV_SafetyI_Type;
                }
                else if (cctv.type === POIManager.PTZ_Type) {
                    sensorType = SDMSMainMenu.CCTV_PTZ_Type;
                }
            }
        }

        const url = '/resource/image/icon/' + sensorType + '.png';
        const sprite = this.addPOI(url, x, y, z, scale, sensorType);

        if (sprite) {
            sprite.name = this.getSensorKey(sensorType, zoneID, sensorID);
            this.sensorPOIs[sprite.name] = sprite;
        }

        return sprite;
    }

    getCCTV(sensorID, zoneID) {
        const selectedNewCCTV = this.contents3D.props.selectedNewCCTV;

        if (selectedNewCCTV && selectedNewCCTV.id === sensorID) {
            return selectedNewCCTV;
        }

        let zone = this.contents3D.props._3dOptions.zones[zoneID];

        if (!zone) {
            zone = this.contents3D.props._3dOptions.outdoorZones[zoneID];
        }

        if (zone?.sensors?.cctv) {
            const cctvCount = zone.sensors.cctv.length;

            for (let i = 0; i < cctvCount; i++) {
                const cctv = zone.sensors.cctv[i];

                if (cctv.id === sensorID) {
                    return cctv;
                }
            }
        }

        return null;
    }

    getSensorIDs(sensorType) {
        const sensorLayer = this.sensorLayers[sensorType];

        if (!sensorLayer) {
            return {};
        }

        const ids = {};
        const childCount = sensorLayer.children.length;

        for (let i = 0; i < childCount; i++) {
            const sensor = sensorLayer.children[i];
            const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(sensor);

            if (sensorID !== null && sensorID !== undefined) {
                ids[sensorID] = sensorID;
            }
        }

        return ids;
    }

    // 층 이동 시 편집 내용이 있다면 적용
    applyToEditSensors(sensors, sensorType, zoneID) {
        const editSensors = this.contents3D?.props?.editModeManager.editSensors[sensorType];
        const deleteCCTVPOIs = this.contents3D?.props?.editModeManager.deleteCCTVPOIs[zoneID];
        const newCCTVPOIs = this.contents3D?.props?.editModeManager.newCCTVPOIs[zoneID];

        const newCCTVList = this.contents3D?.props?.newCCTVList;

        if (sensors === null || sensors === undefined || 
            ((editSensors === null || editSensors === undefined) &&
            (sensorType !== SDMSMainMenu.CCTV_Type || (deleteCCTVPOIs === null || deleteCCTVPOIs === undefined)) &&
            (sensorType !== SDMSMainMenu.CCTV_Type || (newCCTVPOIs === null || newCCTVPOIs === undefined)) )
        )
            return;

        // 새로 추가된 cctv 추가
        if (sensorType === SDMSMainMenu.CCTV_Type) {
            if (newCCTVPOIs !== null && newCCTVPOIs !== undefined && newCCTVList !== null && newCCTVList !== undefined) {
                const poiCount = newCCTVPOIs.length;

                for (let j = 0; j < poiCount; j++) {
                    const poi = newCCTVPOIs[j];
                    const [poiSensorType, poiZoneID, poiSensorID] = SDMS.getSensorInfo(poi);

                    for (let z = 0; z < newCCTVList.length; z++) {
                        const cctvData = newCCTVList[z];

                        if (cctvData.id === poiSensorID) {
                            let cctvSensor = Object.assign({}, cctvData);
                            cctvSensor.zoneID = zoneID;
                            cctvSensor.x = poi.position.x;
                            cctvSensor.y = poi.position.y;
                            cctvSensor.z = poi.position.z;
                            sensors.push(cctvSensor);

                            break;
                        }
                    }

        //            let cctvSensor = {};
        //            cctvSensor.id = poiSensorID;
        //            cctvSensor.type = poiSensorType;
        //            cctvSensor.x = poi.position.x;
        //            cctvSensor.y = poi.position.y;
        //            cctvSensor.z = poi.position.z;

        //            sensors.push(cctvSensor);
                }
            }
        }

        for (let i = 0; i < sensors.length; i++) {
            const sensor = sensors[i];

            // 위치가 수정된 센서가 있다면 위치 수정
            if (editSensors !== null && editSensors !== undefined) {
                for (const editSensorID in editSensors) {
                    const [poi, sensorZoneID, originSensor] = editSensors[editSensorID];

                    if (sensorZoneID === zoneID && sensor.id === originSensor.id) {
                        sensor.x = poi.object.position.x;
                        sensor.y = poi.object.position.y;
                        sensor.z = poi.object.position.z;
                        break;
                    }
                }
            }

            

            // 삭제된 CCTV가 있다면 안보이도록
            if (sensorType === SDMSMainMenu.CCTV_Type) {
                
                if (deleteCCTVPOIs !== null && deleteCCTVPOIs !== undefined) {

                    for (let j = 0; j < deleteCCTVPOIs.length; j++) {
                        const cctv = deleteCCTVPOIs[j];

                        if (sensor.id === cctv.id) {
                            sensor.x = null;
                            sensor.y = null;
                            sensor.z = null;

                            break;
                        }
                    }
                }
            }
        }
    }

    addSensors(sensorType, sensors, scale, zoneID) {
        if (!sensors || !zoneID) {
            return;
        }

        //const urlPath = '/resource/image/icon/';

        const sensorCount = sensors.length;
        let sensorIDs = null;

        for (let i = 0; i < sensorCount; i++) {
            const sensor = sensors[i];

            if (sensor.x === null || sensor.x === undefined ||
                sensor.y === null || sensor.y === undefined ||
                sensor.z === null || sensor.z === undefined) {

                continue;
            }

            let _sensorType = sensorType;

            if (_sensorType === SDMSMainMenu.CCTV_Type) {
                if (sensor.type === POIManager.Safety_I_Type) {
                    _sensorType = SDMSMainMenu.CCTV_SafetyI_Type;
                }
                else if (sensor.type === POIManager.PTZ_Type) {
                    _sensorType = SDMSMainMenu.CCTV_PTZ_Type;
                }
            }

            if (sensorIDs === null) {
                sensorIDs = this.getSensorIDs(_sensorType);
            }

            if (sensorIDs) {
                const sensorID = sensorIDs[sensor.id];

                // 이미 존재하는 POI이기 때문에 다시 만들 필요가 없다.
                if (sensorID !== undefined) {
                    continue;
                }
            }

            const url = this.getSensorImageURL(_sensorType, sensor);
            /*let url = urlPath + sensorType + '.png'
            if (sensor.sensorSubType !== null && sensor.sensorSubType >= 0) {
                url = urlPath + sensorType + sensor.sensorSubType + '.png';
            }*/

            const sprite = this.addPOI(url, sensor.x, sensor.y, sensor.z, scale, _sensorType);

            if (sprite) {
                sprite.name = this.getSensorKey(_sensorType, zoneID, sensor.id);
                this.sensorPOIs[sprite.name] = sprite;

                if (sensor.enabled !== null && sensor.enabled === false) {
                    this.changePoiObject(sprite, false, false);
                }
            }
        }
    }

    getSensorImageURL(sensorType, sensor) {
        const urlPath = '/resource/image/icon/';
        let url = urlPath + sensorType + '.png';

        if (sensor && sensor.sensorSubType !== null && sensor.sensorSubType >= 0) {
            url = urlPath + sensorType + sensor.sensorSubType + '.png';
        }

        return url;
    }

    static isSprite(obj) {
        if (obj.object && obj.object.type === "Sprite") {
            return true;
        }

        return false;
    }

    checkSensors(zoneData, zoneID, sensorData) {
        if (!zoneData.sensors) {
            zoneData.sensors = {};
        }

        if (!zoneData.sensors.fire) {
            zoneData.sensors.fire = [];
        }

        if (!zoneData.sensors.psm) {
            zoneData.sensors.psm = [];
        }

        if (!zoneData.sensors.etc) {
            zoneData.sensors.etc = [];
        }

        if (!zoneData.sensors.cctv) {
            zoneData.sensors.cctv = [];
        }

        if (sensorData.fireSensors) {
            this.checkFireSensors(sensorData.fireSensors, zoneData.sensors.fire);
        }

        if (sensorData.psmSensors) {
            this.checkPSMSensors(sensorData.psmSensors, zoneData.sensors.psm, zoneID);
        }

        /*if (sensorData.etcSensors) {
            this.checkEtcSensors(sensorData.etcSensors, zoneData.sensors.etc);
        }*/

        if (sensorData.cctvs) {
            this.checkCCTVs(sensorData.cctvs, zoneData.sensors.cctv);
        }
    }

    checkCCTVs(srcCCTVs, trgCCTVs) {
        const sensorCount = srcCCTVs.length;
        const targetCount = trgCCTVs.length;

        //const sensorType = SDMSMainMenu.CCTV_Type;
        const targetIndices = {};

        for (let i = 0; i < targetCount; i++) {
            targetIndices[i] = i;
        }

        for (let i = 0; i < sensorCount; i++) {
            const sensor = srcCCTVs[i];
            let _sensor = trgCCTVs[i];

            if (_sensor === null || _sensor === undefined) {
                _sensor = {};
                _sensor.id = sensor.id;
                trgCCTVs.push(_sensor);
            }
            else if (sensor.id !== _sensor.id) {
                _sensor = this.findSensor(sensor.id, trgCCTVs, targetCount);

                if (_sensor === null || _sensor === undefined) {
                    _sensor = {};
                    _sensor.id = sensor.id;
                    trgCCTVs.push(_sensor);
                }
            }

            if (_sensor) {
                _sensor.enabled = sensor.enabled;
                _sensor.isIndoor = sensor.isIndoor;
                _sensor.positionName = sensor.positionName;
                _sensor.name = sensor.name;
                _sensor.zoneID = sensor.zoneID;

                _sensor.bigURL = sensor.bigURL;
                _sensor.cameraName = sensor.cameraName;
                _sensor.channel = sensor.channel;
                _sensor.description = sensor.description;
                _sensor.smallURL = sensor.smallURL;
                _sensor.type = sensor.type;
                _sensor.uniqueKey = sensor.uniqueKey;
                _sensor.url = sensor.url;

                /*if (TextPOIManager.isSameCoord(_sensor.x, sensor.x) === false ||
                    TextPOIManager.isSameCoord(_sensor.y, sensor.y) === false ||
                    TextPOIManager.isSameCoord(_sensor.z, sensor.z) === false) {
                    this.movePOI(sensorType, sensor.zoneID, sensor.id, sensor.x, sensor.y, sensor.z);
                }*/

                _sensor.x = sensor.x;
                _sensor.y = sensor.y;
                _sensor.z = sensor.z;

                targetIndices[i] = null;
            }
        }

        for (let i = targetCount - 1; i >= 0; i--) {
            if (targetIndices[i] !== null) {
                //const sensor = trgCCTVs[targetIndices[i]];
                //this.removePOI(sensorType, sensor.zoneID, sensor.id);
                trgCCTVs.splice(i, 1);
            }
        }
    }

    checkPSMSensors(srcPSMSensors, trgPSMSensors, zoneID) {
        const sensorCount = srcPSMSensors.length;
        const targetCount = trgPSMSensors.length;

        //const sensorType = SDMSMainMenu.PSM_Sensor;
        const targetIndices = {};

        for (let i = 0; i < targetCount; i++) {
            targetIndices[i] = i;
        }

        for (let i = 0; i < sensorCount; i++) {
            const sensor = srcPSMSensors[i];
            const _sensor = trgPSMSensors[i];

            if (!_sensor) {
                _sensor = {};
                _sensor.id = sensor.id;
                trgPSMSensors.push(_sensor);
            }
            else if (sensor.id !== _sensor.id) {
                _sensor = this.findSensor(sensor.id, trgPSMSensors, targetCount);

                if (!_sensor) {
                    _sensor = {};
                    _sensor.id = sensor.id;
                    trgPSMSensors.push(_sensor);
                }
            }

            if (_sensor) {
                _sensor.department = sensor.department;
                _sensor.departmentPhoneNumber = sensor.departmentPhoneNumber;
                _sensor.enabled = sensor.enabled;
                _sensor.isIndoor = sensor.isIndoor;
                _sensor.positionName = sensor.positionName;
                _sensor.name = sensor.name;
                _sensor.sensorSubType = sensor.sensorSubType;
                _sensor.sensorTagInfoID = sensor.sensorTagInfoID;
                _sensor.sensorZoneID = sensor.sensorZoneID;
                _sensor.tagNo = sensor.tagNo;

                _sensor.equipZoneID = sensor.equipZoneID;
                _sensor.status = sensor.status;
                _sensor.limitLevel1 = sensor.limitLevel1;
                _sensor.limitLevel2 = sensor.limitLevel2;
                _sensor.limitLevel3 = sensor.limitLevel3;
                _sensor.useLimitLevel1 = sensor.useLimitLevel1;
                _sensor.useLimitLevel2 = sensor.useLimitLevel2;
                _sensor.useLimitLevel3 = sensor.useLimitLevel3;

                /*if (TextPOIManager.isSameCoord(_sensor.x, sensor.x) === false ||
                    TextPOIManager.isSameCoord(_sensor.y, sensor.y) === false ||
                    TextPOIManager.isSameCoord(_sensor.z, sensor.z) === false) {
                    this.movePOI(sensorType, zoneID, sensor.id, sensor.x, sensor.y, sensor.z);
                }*/

                _sensor.x = sensor.x;
                _sensor.y = sensor.y;
                _sensor.z = sensor.z;

                targetIndices[i] = null;
            }
        }

        for (let i = targetCount - 1; i >= 0; i--) {
            if (targetIndices[i] !== null) {
                //const sensor = trgPSMSensors[targetIndices[i]];
                //this.removePOI(sensorType, zoneID, sensor.id);
                trgPSMSensors.splice(i, 1);
            }
        }
    }

    checkFireSensors(srcFireSensors, trgFireSensors) {
        const sensorCount = srcFireSensors.length;
        const targetCount = trgFireSensors.length;

        //const sensorType = SDMSMainMenu.Fire_Sensor;
        const targetIndices = {};

        for (let i = 0; i < targetCount; i++) {
            targetIndices[i] = i;
        }

        for (let i = 0; i < sensorCount; i++) {
            const sensor = srcFireSensors[i];
            let _sensor = trgFireSensors[i];

            if (!_sensor) {
                _sensor = {};
                _sensor.id = sensor.id;
                trgFireSensors.push(_sensor);
            }
            else if (sensor.id !== _sensor.id) {
                _sensor = this.findSensor(sensor.id, trgFireSensors, targetCount);

                if (!_sensor) {
                    _sensor = {};
                    _sensor.id = sensor.id;
                    trgFireSensors.push(_sensor);
                }
            }

            if (_sensor) {
                _sensor.department = sensor.department;
                _sensor.departmentPhoneNumber = sensor.departmentPhoneNumber;
                _sensor.enabled = sensor.enabled;
                _sensor.isIndoor = sensor.isIndoor;
                _sensor.positionName = sensor.positionName;
                _sensor.name = sensor.name;
                _sensor.sensorSubType = sensor.sensorSubType;
                _sensor.sensorTagInfoID = sensor.sensorTagInfoID;
                _sensor.sensorZoneID = sensor.sensorZoneID;
                _sensor.tagNo = sensor.tagNo;
                _sensor.zoneID = sensor.zoneID;

                /*if (TextPOIManager.isSameCoord(_sensor.x, sensor.x) === false ||
                    TextPOIManager.isSameCoord(_sensor.y, sensor.y) === false ||
                    TextPOIManager.isSameCoord(_sensor.z, sensor.z) === false) {
                    this.movePOI(sensorType, sensor.zoneID, sensor.id, sensor.x, sensor.y, sensor.z);
                }*/

                _sensor.x = sensor.x;
                _sensor.y = sensor.y;
                _sensor.z = sensor.z;

                targetIndices[i] = null;
            }
        }

        for (let i = targetCount - 1; i >= 0; i--) {
            if (targetIndices[i] !== null) {
                //const sensor = trgFireSensors[targetIndices[i]];
                //this.removePOI(sensorType, sensor.zoneID, sensor.id);
                trgFireSensors.splice(i, 1);
            }
        }
    }

    findSensor(id, sensors, sensorCount) {
        for (let i = 0; i < sensorCount; i++) {
            const sensor = sensors[i];

            if (sensor.id === id) {
                return sensor;
            }
        }

        return null;
    }

    getSensorLayer(sensorType) {
        if (!this.scene) {
            return null;
        }

        let layer = this.sensorLayers[sensorType];

        if (!layer) {
            layer = new THREE.Object3D();
            layer.matrixAutoUpdate = false;
            layer.name = "sensors_" + sensorType;

            this.sensorLayers[sensorType] = layer;
            this.scene.add(layer);
        }

        return layer;
    }

    setVisibleSensorTypes(visibleSensorTypes) {
        for (const sensorType in visibleSensorTypes) {
            const visible = visibleSensorTypes[sensorType];
            const layer = this.getSensorLayer(sensorType);

            if (layer) {
                layer.visible = visible;
            }
        }
    }

    showTempCCTVPOI(isIndoor, visible, x, y, z) {
        let tempPOI = null;

        if (isIndoor) {
            if (this.tempOutdoorCCTVPOI) {
                this.tempOutdoorCCTVPOI.visible = false;
            }

            tempPOI = this.tempIndoorCCTVPOI;
        }
        else {
            if (this.tempIndoorCCTVPOI) {
                this.tempIndoorCCTVPOI.visible = false;
            }

            tempPOI = this.tempOutdoorCCTVPOI;
        }

        if (tempPOI) {
            if (x !== null && x !== undefined && z !== null && z !== undefined) {
                if (y === null || y === undefined) {
                    tempPOI.position.set(x, tempPOI.position.y, z);
                }
                else {
                    tempPOI.position.set(x, y, z);
                }
            }

            tempPOI.visible = visible;
        }
    }

    putTempCCTV(event, camera, isIndoor, zoneID, cctv, _3dOptions) {
        let tempPOI = isIndoor ? this.tempIndoorCCTVPOI : this.tempOutdoorCCTVPOI;
        let height = this.getOtherPOIHeight(isIndoor, zoneID, _3dOptions);

        if (tempPOI?.visible && cctv) {
            if (height === null) {
                const x = event.nativeEvent.offsetX;
                const y = event.nativeEvent.offsetY;
                const mouse = new THREE.Vector2((x / window.innerWidth) * 2 - 1, -(y / window.innerHeight) * 2 + 1);

                const raycaster = new THREE.Raycaster();
                raycaster.setFromCamera(mouse, camera);

                const intersects = raycaster.intersectObjects(this.scene.children, true);
                const intersectCount = intersects.length;

                let maxHeight = null;

                for (let i = 0; i < intersectCount; i++) {
                    const intersect = intersects[i];

                    if (intersect.object.visible === false || (intersect.object.parent && intersect.object.parent.visible === false)) {
                        continue;
                    }

                    if (!intersect.object.geometry.boundingBox) {
                        intersect.object.geometry.computeBoundingBox();
                    }

                    const boundingBox = intersect.object.geometry.boundingBox;

                    if (boundingBox) {
                        const _height = boundingBox.max.y * intersect.object.scale.y + intersect.point.y;

                        if (maxHeight === null || maxHeight < _height) {
                            maxHeight = _height;
                        }
                    }
                }

                if (maxHeight !== null) {
                    height = maxHeight - POIManager.MinusHeight;
                }
            }

            if (height !== null) {
                let sensorType = SDMSMainMenu.CCTV_Type;

                if (cctv.type === POIManager.Safety_I_Type) {
                    sensorType = SDMSMainMenu.CCTV_SafetyI_Type;
                }

                const poi = this.addSensor(sensorType, cctv.id, tempPOI.position.x, height, tempPOI.position.z, zoneID, isIndoor);

                if (poi) {
                    return poi;
                }
            }
        }

        return null;
    }

    deleteCCTV(poi, _3dOptions) {
        const poiInfo = this.getSensorInfo(poi.object.name);

        if (poiInfo === null)
            return false;

        const sensorType = poiInfo[0];

        if (sensorType === SDMSMainMenu.CCTV_Type ||
            sensorType === SDMSMainMenu.CCTV_SafetyI_Type) {
            return this.removePOI(sensorType, poiInfo[1], poiInfo[2]);
        }

        return false;
    }

    // 기존에 배치된 POI들의 높이값을 얻어온다.
    getOtherPOIHeight(isIndoor, zoneID, _3dOptions) {
        let zone = isIndoor ? _3dOptions.zones[zoneID] : _3dOptions.outdoorZones[zoneID];

        if (!zone) {
            return null;
        }

        if (zone.datas?.poiElevation) {
            return zone.datas.poiElevation;
        }

        if (!zone.sensors) {
            return null;
        }

        for (const sensorType in zone.sensors) {
            const sensors = zone.sensors[sensorType];
            const sensorCount = sensors.length;

            for (let i = 0; i < sensorCount; i++) {
                const sensor = sensors[i];
                return sensor.y;
            }
        }

        return null;
    }
}