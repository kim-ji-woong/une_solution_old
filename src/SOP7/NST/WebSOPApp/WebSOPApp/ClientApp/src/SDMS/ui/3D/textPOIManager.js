import { SDMSController } from "../../services/sdmsController";
import * as THREE from "three/build/three.module.js";
import Geometry from '../../../Common/util/Geometry.js';
import SDMSMainMenu from "../../data/sdmsMainMenu";
//const THREE = require('three/build/three.module.js');

/*type rgbaColor = {
    r: number,
    g: number,
    b: number,
    a: number
};

type nullableNumber = number | null | undefined;

export interface IScene {
    add(param: object): void;
}*/

export class TextPOIManager {
    static BuildingGroupTextDistance = 500;
    static BuildingGroupFontSize = 36;
    static BuildingFontSize = 12;
    static EquipZoneFontSize = 12;

    scene = null;
    buildingGroupTextVisible = false;
    buildingGroupText/*: { [buildingGroupName: string]: [THREE.Sprite, string] }*/ = {};
    buildingTextVisible = false;
    buildingText/*: { [buildingGroupName: string]: { [buildingName: string]: THREE.Sprite } }*/ = {};
    equipZoneText/*: { [zoneID: number]: { [equipZoneID: number]: THREE.Sprite } }*/ = {};
    // 최근에 실내에서 표시되었던 EquipZone Text Sprite
    prevEquipZoneSprites/*: THREE.Sprite[]*/ = [];
    textLayer = null;

    _2TextWidth = null;

    get Scene() {
        return this.scene;
    }

    set Scene(scene) {
        this.scene = scene;
    }

    addBuildingGroupText(buildingGroups/*: Array<[string, string, string, number, number, number]>*/)/*:void*/ {
        const buildingGroupCount = buildingGroups.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = buildingGroups[i];
            const buildingGroupName = buildingGroup[0];
            const displayText = buildingGroup[1];
            const x = buildingGroup[3];
            const y = buildingGroup[4];
            const z = buildingGroup[5];
            const id = buildingGroup[6];

            const sprite = this.makeBuildingGroupText(displayText, id, x, y, z, TextPOIManager.BuildingGroupFontSize);

            if (sprite) {
                this.buildingGroupText[buildingGroupName] = [sprite, displayText];
            }
        }

        this.buildingGroupTextVisible = true;
    }

    addBuildingText(buildings/*: { [buildingGroupName: string]: {} }*/)/*: void*/ {
        for (const buildingGroupName in buildings) {
            const buildingGroup = buildings[buildingGroupName];

            const buildingSprites/*: { [buildingName: string]: THREE.Sprite }*/ = {};
            this.buildingText[buildingGroupName] = buildingSprites;

            for (const buildingName in buildingGroup) {
                const building = buildingGroup[buildingName];// as [number, string, string, nullableNumber, nullableNumber, nullableNumber, object];

                const displayText = building[1];
                const x = building[3];
                const y = building[4];
                const z = building[5];
                const id = building[0];

                if (x !== null && x !== undefined && y !== null && y !== undefined && z !== null && z !== undefined) {
                    const sprite = this.makeBuildingText(SDMSMainMenu.BuildingNameText, displayText, -1, id, x, y, z, TextPOIManager.BuildingFontSize);

                    if (sprite) {
                        sprite.visible = false;
                        buildingSprites[buildingName] = sprite;
                    }
                }
            }
        }

        this.buildingTextVisible = false;
    }

    addEquipZoneText(zoneID/*: number*/, equipZones/*: { [equipZoneID: number]: [number, string, THREE.Vector3] }*/)/*: void*/ {
        const equipZoneSprites/*: { [equipZoneID: number]: THREE.Sprite }*/ = {};
        this.equipZoneText[zoneID] = equipZoneSprites;

        for (const equipZoneID in equipZones) {
            const equipZone = equipZones[equipZoneID];

            if (equipZone && equipZone[1].length > 0 && equipZone[2] !== null) {
                //const id = equipZone[0];
                const equipZoneName = equipZone[1];
                const textCenter = equipZone[2];

                const sprite = this.makeBuildingText(SDMSMainMenu.EquipZoneNameText, equipZoneName, zoneID, equipZoneID, textCenter.x, textCenter.y, textCenter.z, TextPOIManager.EquipZoneFontSize);

                if (sprite) {
                    sprite.scale.set(12.5, 6.25, 1.0);
                    sprite.visible = false;
                    equipZoneSprites[equipZoneID] = sprite;
                }
            }
        }
    }

    makeBuildingGroupText(text/*: string*/, id/*: number*/, x/*: number*/, y/*: number*/, z/*: number*/, fontSize/*: number*/)/*: THREE.Sprite | null*/ {
        const backgroundColor = { r: 0, g: 0, b: 0, a: 0.7 };
        const borderColor = { r: 63, g: 108, b: 219, a: 1.0 };
        const textColor = { r: 255, g: 255, b: 255, a: 1.0 };
        const borderThickness = 1;
        return this.addText(SDMSMainMenu.BuildingGroupNameText, text, -1, id, x, y, z, fontSize, backgroundColor, borderColor, textColor, borderThickness);
    }

    makeBuildingText(textTag/*: string*/, text/*: string*/, zoneID/*: number*/, id/*: number*/, x/*: number*/, y/*: number*/, z/*: number*/, fontSize/*: number*/)/*: THREE.Sprite | null*/ {
        const backgroundColor = { r: 0, g: 0, b: 0, a: 0.7 };
        const borderColor = { r: 63, g: 108, b: 219, a: 1.0 };
        const textColor = { r: 255, g: 255, b: 255, a: 1.0 };
        const borderThickness = 1;
        return this.addText(textTag, text, zoneID, id, x, y, z, fontSize, backgroundColor, borderColor, textColor, borderThickness);
    }

    setBuildingGroupTextVisible(visible/*: boolean*/)/*: void*/ {
        for (const buildingGroupName in this.buildingGroupText) {
            const [sprite, spriteText] = this.buildingGroupText[buildingGroupName];

            if (sprite && spriteText) {
                sprite.visible = visible;
            }
        }

        this.buildingGroupTextVisible = visible;
    }

    showBuildingGroupText(zoomValue/*: number*/, isIndoor/*: boolean*/)/*: boolean*/ {
        if (this.buildingGroupTextVisible) {
            if (isIndoor || zoomValue < TextPOIManager.BuildingGroupTextDistance) {
                this.setBuildingGroupTextVisible(false);
                return false;
            }
        }
        else {
            if (!isIndoor && zoomValue >= TextPOIManager.BuildingGroupTextDistance) {
                this.setBuildingGroupTextVisible(true);
            }
        }

        return true;
    }

    showBuildingText(zoomValue/*: number*/, isIndoor/*: boolean*/)/*: void*/ {
        if (this.buildingTextVisible) {
            if (isIndoor || zoomValue >= TextPOIManager.BuildingGroupTextDistance) {
                this.setBuildingTextVisible(false);
            }
        }
        else {
            if (!isIndoor && zoomValue < TextPOIManager.BuildingGroupTextDistance) {
                this.setBuildingTextVisible(true);
            }
        }
    }

    showEquipZoneSprites(zoneID/*: number*/)/*: void*/ {
        const equipZoneSprites = this.equipZoneText[zoneID];

        if (equipZoneSprites) {
            for (const equipZoneID in equipZoneSprites) {
                const sprite = equipZoneSprites[equipZoneID];
                sprite.visible = true;
                this.prevEquipZoneSprites.push(sprite);
            }
        }
    }

    hideEquipZoneSprites() {
        for (let i = 0; i < this.prevEquipZoneSprites.length; i++) {
            const sprite = this.prevEquipZoneSprites[i];
            sprite.visible = false;
        }

        this.prevEquipZoneSprites = [];
    }

    setBuildingTextVisible(visible/*: boolean*/)/*: void*/ {
        for (const buildingGroupName in this.buildingText) {
            const buildingGroup = this.buildingText[buildingGroupName];

            for (const buildingName in buildingGroup) {
                const text = buildingGroup[buildingName];
                text.visible = visible;
            }
        }

        this.buildingTextVisible = visible;
    }

    getBuildingTextSprite(buildingGroupName/*: string*/, buildingName/*: string*/)/*: THREE.Sprite | null*/ {
        const buildingGroup = this.buildingText[buildingGroupName];

        if (buildingGroup) {
            const sprite = buildingGroup[buildingName];
            return sprite;
        }

        return null;
    }

    clear() {
        this.buildingGroupText = {};
    }

    async moveEquipZoneNameText(zoneID/*: number*/, equipZoneID/*: number*/, equipZoneName/*: string*/, x/*: number*/, y/*: number*/, z/*: number*/, successMethod/*: (zoneID: number, equipZoneID: number, equipZoneName: string, x: number, y: number, z: number) => void*/) {
        const result = await SDMSController.requestMoveEquipZoneNameText(equipZoneID, equipZoneName, x, y, z);// as [boolean, string];
        const success = result[0];
        const message = result[1];

        if (success) {
            const equipZoneSprites = this.equipZoneText[zoneID];

            if (equipZoneSprites) {
                const sprite = equipZoneSprites[equipZoneID];

                if (sprite) {
                    sprite.position.x = x;
                    sprite.position.y = y;
                    sprite.position.z = z;
                }
            }

            successMethod(zoneID, equipZoneID, equipZoneName, x, y, z);
        }
        else {
            alert(message);
        }
    }

    // 건물그룹, 건물의 이름과 좌표를 새로 얻어온다.
    async updateOuterDatas(_3dOptions/*: object*/, poiManager/*: POIManager*/) {
        const result = await SDMSController.requestOuterDatas();// as [object, object, string];

        if (result && result[0] && result[1]) {
            const buildingGroups = result[0];
            const outdoorZones = result[1];

            this.checkBuildingGroups(_3dOptions, buildingGroups, this.buildingGroupText);
            this.checkBuildings(_3dOptions, buildingGroups, this.buildingText);

            const zoneCount = outdoorZones.length;

            for (let i = 0; i < zoneCount; i++) {
                const zone = outdoorZones[i];
                const zoneData = this.getZone(zone.id, _3dOptions);

                if (zoneData) {
                    poiManager.checkSensors(zoneData, zone.id, zone.sensors);
                }
            }
        }
    }

    async updateIndoorDatas(zoneID/*: number*/, _3dOptions/*: object*/, poiManager/*: POIManager*/) {
        const zoneData = this.getZone(zoneID, _3dOptions);

        if (zoneData) {
            const [result, message] = await SDMSController.requestIndoorDatas(zoneID);

            if (result === null) {
                console.log(message);
            }
            else {
                if (result.equipZones) {
                    this.checkEquipZones(result.equipZones, zoneData.equipZones, zoneID);
                }

                poiManager.checkSensors(zoneData, zoneID, result);
            }
        }
    }

    checkEquipZones(equipZoneDatas, equipZones, zoneID) {
        const equipZoneCount = equipZoneDatas.length;
        const equipZoneSprites = this.equipZoneText[zoneID];

        if (!equipZoneSprites) {
            return;
        }

        for (let i = 0; i < equipZoneCount; i++) {
            const equipZoneData = equipZoneDatas[i];

            const sprite = equipZoneSprites[equipZoneData.id];
            const equipZone = equipZones[equipZoneData.id];

            if (sprite && equipZone) {
                const prevPos = equipZone[2];

                if (!equipZoneData.textCenter && prevPos) {
                    this.scene.remove(sprite);
                    delete equipZoneSprites[equipZoneData.id];
                    equipZone[2] = null;
                }
                else if ((equipZoneData.textCenter && !prevPos) ||
                    equipZoneData.zoneName !== equipZone[1]) {
                    this.updateEquipZoneText(equipZoneData, equipZone, equipZoneSprites, sprite);
                }
                else if (equipZoneData.textCenter && prevPos &&
                    (TextPOIManager.isSameCoord(equipZoneData.textCenter.x, prevPos.x) == false ||
                        TextPOIManager.isSameCoord(equipZoneData.textCenter.y, prevPos.y) == false ||
                        TextPOIManager.isSameCoord(equipZoneData.textCenter.z, prevPos.z) == false)) {
                    this.updateEquipZoneText(equipZoneData, equipZone, equipZoneSprites, sprite);
                }
            }
        }
    }

    getZone(zoneID/*: number*/, _3dOptions/*: object*/)/*: [floorIndex: number, buildingID: number, zoneName: string, displayText: string, x: number, y: number, z: number, sensors: object]*/ {
        let zoneData = _3dOptions.zones[zoneID];

        if (zoneData) {
            return zoneData;
        }
        else {
            zoneData = _3dOptions.outdoorZones[zoneID];

            if (zoneData) {
                return zoneData;
            }
        }

        return null;
    }

    checkBuildings(_3dOptions, buildingGroups, buildingTextSprite) {
        const buildingGroupCount = buildingGroups.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroupName = buildingGroups[i].groupName;
            const buildingDatas = buildingGroups[i].buildingDatas;
            const buildingCount = buildingDatas.length;

            const buildings = _3dOptions.buildings[buildingGroupName];
            const buildingSprites = buildingTextSprite[buildingGroupName];

            if (buildings && buildingSprites) {
                for (let j = 0; j < buildingCount; j++) {
                    const buildingData = buildingDatas[j];
                    const buildingSprite = buildingSprites[buildingData.buildingName];

                    if (!buildingSprite) {
                        continue;
                    }

                    const building = buildings[buildingData.buildingName]; // as [id: number, buildingName: string, displayText: string, x, y, z];

                    if (!building) {
                        continue;
                    }

                    if (building[1] !== buildingData.displayText ||
                        TextPOIManager.isSameCoord(building[3], buildingData.textCenter.x) === false ||
                        TextPOIManager.isSameCoord(building[4], buildingData.textCenter.y) === false ||
                        TextPOIManager.isSameCoord(building[5], buildingData.textCenter.z) === false) {
                        const buildingIDData = _3dOptions.buildingIDs[building[0]];
                        const allBuildingData = _3dOptions.allBuildings[buildingData.buildingName];
                        this.updateBuildingText(buildingData, buildingIDData, allBuildingData, building, buildingSprites, buildingSprite);
                    }
                }
            }
        }
    }

    static isSameCoord(data1/*: number*/, data2/*: number*/)/*: boolean*/ {
        const diff = data1 - data2;

        if (diff > -0.1 && diff < 0.1) {
            return true;
        }

        return false;
    }

    updateBuildingText(buildingData, buildingIDData, allBuildingData, building, buildingSprites, oldSprite) {
        const pos = buildingData.textCenter;
        const sprite = this.makeBuildingText(buildingData.displayText, pos.x, pos.y, pos.z, TextPOIManager.BuildingFontSize);

        if (sprite) {
            sprite.visible = oldSprite.visible;
            this.scene.remove(oldSprite);

            buildingSprites[buildingData.buildingName] = sprite;

            allBuildingData[2] = buildingData.displayText;
            buildingIDData[2] = buildingData.displayText;

            building[1] = buildingData.displayText;
            building[3] = pos.x;
            building[4] = pos.y;
            building[5] = pos.z;
        }
    }

    updateEquipZoneText(equipZoneData, equipZone, equipZoneSprites, oldSprite) {
        const pos = equipZoneData.textCenter;
        const sprite = this.makeBuildingText(equipZoneData.zoneName, pos.x, pos.y, pos.z, TextPOIManager.EquipZoneFontSize);

        if (sprite) {
            this.scene.remove(oldSprite);

            sprite.scale.set(12.5, 6.25, 1.0);

            equipZoneSprites[equipZoneData.id] = sprite;
            equipZone[1] = equipZoneData.zoneName;
            equipZone[2] = { x: pos.x, y: pos.y, z: pos.z };
        }
    }

    checkBuildingGroups(_3dOptions, buildingGroups, buildingGroupTextSprite) {
        const buildingGroupCount = buildingGroups.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const _buildingGroup = buildingGroups[i];
            const spriteData = buildingGroupTextSprite[_buildingGroup.groupName];

            if (spriteData) {
                const buildingGroup = this.getBuildingGroup(_3dOptions, _buildingGroup.groupName); // as [buildingGroupName: string, displayText: string, boundingBoxName: string, x, y, z]

                if (buildingGroup && buildingGroup[1]) {
                    if (_buildingGroup.displayText !== buildingGroup[1] ||
                        TextPOIManager.isSameCoord(_buildingGroup.textCenter.x, buildingGroup[3]) === false ||
                        TextPOIManager.isSameCoord(_buildingGroup.textCenter.y, buildingGroup[4]) === false ||
                        TextPOIManager.isSameCoord(_buildingGroup.textCenter.z, buildingGroup[5]) === false) {

                        const pos = _buildingGroup.textCenter;
                        const sprite = this.makeBuildingGroupText(_buildingGroup.displayText, pos.x, pos.y, pos.z, TextPOIManager.BuildingGroupFontSize);

                        if (sprite) {
                            this.scene.remove(spriteData[0]);

                            spriteData[0] = sprite;
                            spriteData[1] = _buildingGroup.displayText;
                            buildingGroup[1] = _buildingGroup.displayText;
                            buildingGroup[3] = pos.x;
                            buildingGroup[4] = pos.y;
                            buildingGroup[5] = pos.z;
                        }
                    }
                }
            }
        }
    }

    getBuildingGroup(_3dOptions, buildingGroupName) {
        const buildingGroupCount = _3dOptions.buildingGroups.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = _3dOptions.buildingGroups[i];

            if (buildingGroup && buildingGroup.length >= 6 && buildingGroup[0] === buildingGroupName) {
                return buildingGroup;
            }
        }

        return null;
    }

    addText(tag/*: string*/, text/*: string*/, zoneID/*: number*/, id/*: number*/, x/*: number*/, y/*: number*/, z/*: number*/, fontSize/*: number*/, backgroundColor/*: rgbaColor*/, borderColor/*: rgbaColor*/, textColor/*: rgbaColor*/, borderThickness/*: number*/)/*: THREE.Sprite | null*/ {
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');

        if (context === null) {
            return null;
        }

        const fontFace = 'malgun gothic';
        //const fontFace = 'Arial';

        context.font = fontSize + "px " + fontFace;
        //context.font = "Bold " + fontSize + "px " + fontFace;
        context.fillStyle = "rgba(" + backgroundColor.r + "," + backgroundColor.g + "," + backgroundColor.b + "," + backgroundColor.a + ")";
        // border color
        context.strokeStyle = "rgba(" + borderColor.r + "," + borderColor.g + "," + borderColor.b + "," + borderColor.a + ")";
        context.lineWidth = borderThickness;

        const metrics = context.measureText(text);
        const width = metrics.width + 10;

        let w = width + borderThickness;
        const originWidth = w;

        if (tag === SDMSMainMenu.BuildingGroupNameText) {
            if (text.length === 2) {
                if (!this._2TextWidth) {
                    this._2TextWidth = w + 10;
                }

                w = this._2TextWidth;
            }
            else if (text.length < 2) {
                w = this._2TextWidth;
            }
        }

        const h = fontSize * 1.4 + borderThickness;
        const rectX = (canvas.width - w - borderThickness) / 2;
        const rectY = (canvas.height - h - borderThickness) / 2;

        this.roundRect(context, rectX, rectY, w, h, 6);
        //Contents3D.roundRect(context, borderThickness / 2, borderThickness / 2, width + borderThickness, fontSize * 1.4 + borderThickness, 6);

        // text color
        context.fillStyle = "rgba(" + textColor.r + "," + textColor.g + "," + textColor.b + "," + textColor.a + ")";
        //context.fillStyle = "rgba(0, 0, 0, 1.0)";

        const textY = rectY + borderThickness;
        // metrics.width보다 10만큼 크게 잡았으니 5만큼 띄워서 시작한다.
        context.fillText(text, rectX + 5 + (w - originWidth) / 2, rectY + fontSize);
        //context.fillText(text, borderThickness + 5, fontSize + borderThickness);

        // canvas contents will be used for a texture
        const texture = new THREE.Texture(canvas)
        texture.needsUpdate = true;

        // const spriteAlignment = THREE.SpriteAlignment.topLeft;

        const spriteMaterial = new THREE.SpriteMaterial(
            { map: texture/*, useScreenCoordinates: false, alignment: spriteAlignment*/ });
        const sprite = new THREE.Sprite(spriteMaterial);
        sprite.scale.set(100, 50, 1.0);

        //sprite.material.depthWrite = false;
        //sprite.material.depthTest = false;
        sprite.position.x = x;
        sprite.position.y = y;
        sprite.position.z = z;
        sprite.name = tag + "_" + zoneID + "_" + id;
        //sprite.name = "text_" + id + "_" + text;

        sprite.userData.boundingBox = {
            tl: {
                x: x - rectX / 20,
                z: z - rectY / 20
            },
            br: {
                x: x + rectX / 20,
                z: z + rectY / 20
            }
        };

        if (!this.textLayer) {
            this.textLayer = new THREE.Object3D();
            this.textLayer.matrixAutoUpdate = false;
            this.textLayer.name = "textLayer";

            this.scene.add(this.textLayer);
        }

        if (this.scene !== null) {
            this.textLayer.add(sprite);
            //this.scene.add(sprite);
        }

        return sprite;
    }

    roundRect(context/*: CanvasRenderingContext2D*/, x/*: number*/, y/*: number*/, w/*: number*/, h/*: number*/, r/*: number*/)/*: void*/ {
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

    setVisible(visible) {
        if (this.scene) {
            if (!this.textLayer) {
                this.textLayer = new THREE.Object3D();
                this.textLayer.matrixAutoUpdate = false;
                this.textLayer.name = "textLayer";

                this.scene.add(this.textLayer);
            }

            this.textLayer.visible = visible;
        }
    }

    static isTextPOI(sensorType) {
        if (sensorType.startsWith("text")) {
            return true;
        }

        return false;
    }

    static hitTest(textPoi, x, z) {
        const boundingBox = textPoi.object.userData.boundingBox;

        if (!boundingBox || !boundingBox.tl || !boundingBox.br) {
            return false;
        }

        if (x >= boundingBox.tl.x && x <= boundingBox.br.x &&
            z >= boundingBox.tl.z && z <= boundingBox.br.z) {
            return true;
        }

        return false;
    }

    // raycaster를 이용하여 hitTest를 실시한다.
    static hitTest3D(textPoi, rayCenter, rayLeft, rayTop) {
        const boundingBox = textPoi.object.userData.boundingBox;

        if (!boundingBox) {
            return false;
        }

        const vCameraOrigin = rayCenter.ray.origin;
        const vCameraDir = rayCenter.ray.direction;
        const vCameraLook = new THREE.Vector3(vCameraOrigin.x + vCameraDir.x * 100, vCameraOrigin.y + vCameraDir.y * 100, vCameraOrigin.z + vCameraDir.z * 100);

        const vPoiPos = textPoi.object.position;
        const [posX, posY, posZ] = Geometry.getNearestVertex3(vPoiPos.x, vPoiPos.y, vPoiPos.z, vCameraOrigin.x, vCameraOrigin.y, vCameraOrigin.z, vCameraLook.x, vCameraLook.y, vCameraLook.z, true);
        const vMousePos = new THREE.Vector3(posX, posY, posZ);

        const vLeftDir = rayLeft.ray.direction;
        const vTopDir = rayTop.ray.direction;

        const [leftX, leftY, leftZ] = TextPOIManager.getLeftVertex(vCameraOrigin, vMousePos, vLeftDir);
        const [topX, topY, topZ] = TextPOIManager.getLeftVertex(vCameraOrigin, vMousePos, vTopDir);

        const width = boundingBox.br.x - boundingBox.tl.x;
        const height = boundingBox.br.z - boundingBox.tl.z;

        const [leftTargetX, leftTargetY, leftTargetZ] = Geometry.getLinearVertex3(vMousePos.x, vMousePos.y, vMousePos.z, leftX, leftY, leftZ, width / 2);
        const [topTargetX, topTargetY, topTargetZ] = Geometry.getLinearVertex3(vMousePos.x, vMousePos.y, vMousePos.z, topX, topY, topZ, height / 2);

        const vPoiLeft = new THREE.Vector3(leftTargetX - vMousePos.x + vPoiPos.x, leftTargetY - vMousePos.y + vPoiPos.y, leftTargetZ - vMousePos.z + vPoiPos.z);
        const vPoiTop = new THREE.Vector3(topTargetX - vMousePos.x + vPoiPos.x, topTargetY - vMousePos.y + vPoiPos.y, topTargetZ - vMousePos.z + vPoiPos.z);

        const vPoiTL = new THREE.Vector3(vPoiLeft.x + vPoiTop.x - vPoiPos.x, vPoiLeft.y + vPoiTop.y - vPoiPos.y, vPoiLeft.z + vPoiTop.z - vPoiPos.z);
        const vPoiBL = new THREE.Vector3(vPoiLeft.x * 2 - vPoiTL.x, vPoiLeft.y * 2 - vPoiTL.y, vPoiLeft.z * 2 - vPoiTL.z);
        const vPoiBR = new THREE.Vector3(vPoiPos.x * 2 - vPoiTL.x, vPoiPos.y * 2 - vPoiTL.y, vPoiPos.z * 2 - vPoiTL.z);

        return TextPOIManager._hitTest3D(vPoiTL, vPoiBL, vPoiBR, vMousePos);
    }

    static _hitTest3D(vTL, vBL, vBR, vPos) {
        const vTR = new THREE.Vector3(vTL.x + vBR.x - vBL.x, vTL.y + vBR.y - vBL.y, vTL.z + vBR.z - vBL.z);

        const width = Geometry.getDistance3(vBR.x, vBR.y, vBR.z, vBL.x, vBL.y, vBL.z);
        const height = Geometry.getDistance3(vTL.x, vTL.y, vTL.z, vBL.x, vBL.y, vBL.z);

        const len1 = Geometry.getDistanceFromLine3(vPos.x, vPos.y, vPos.z, vTL.x, vTL.y, vTL.z, vBL.x, vBL.y, vBL.z, false);

        if (len1 > width) {
            return false;
        }

        const len2 = Geometry.getDistanceFromLine3(vPos.x, vPos.y, vPos.z, vBL.x, vBL.y, vBL.z, vBR.x, vBR.y, vBR.z, false);

        if (len2 > height) {
            return false;
        }

        const len3 = Geometry.getDistanceFromLine3(vPos.x, vPos.y, vPos.z, vBR.x, vBR.y, vBR.z, vTR.x, vTR.y, vTR.z, false);

        if (len3 > width) {
            return false;
        }

        const len4 = Geometry.getDistanceFromLine3(vPos.x, vPos.y, vPos.z, vTR.x, vTR.y, vTR.z, vTL.x, vTL.y, vTL.z, false);

        if (len4 > height) {
            return false;
        }

        return true;
    }

    static getLeftVertex(vOrigin, vCenter, vDir) {
        const vDirectionEnd = new THREE.Vector3(vOrigin.x + vDir.x * 100, vOrigin.y + vDir.y * 100, vOrigin.z + vDir.z * 100);
        const theta = Geometry.getAngle3(vCenter.x, vCenter.y, vCenter.z, vOrigin.x, vOrigin.y, vOrigin.z, vDirectionEnd.x, vDirectionEnd.y, vDirectionEnd.z);

        const len = Geometry.getDistance3(vOrigin.x, vOrigin.y, vOrigin.z, vCenter.x, vCenter.y, vCenter.z);
        const targetLength = len / Math.cos(theta);

        return Geometry.getLinearVertex3(vOrigin.x, vOrigin.y, vOrigin.z, vDirectionEnd.x, vDirectionEnd.y, vDirectionEnd.z, targetLength);
    }
}

export default TextPOIManager;