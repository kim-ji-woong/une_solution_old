import * as Common from './common';
import Vertex2D from './Vertex2D';

export default class Geometry {
    static Tolerance: number = 0.001;

    static stringToVertexList(str: string): Vertex2D[] {
        const tokens = str.trim().split(' ');
        const tokenCount = tokens.length;

        const vertexList = [];

        for (let i = 0; i < tokenCount; i++) {
            const coord = tokens[i].trim().split(',');

            if (coord.length !== 2) {
                continue;
            }

            const x = parseFloat(coord[0].trim());
            const y = parseFloat(coord[1].trim());

            if (x !== NaN && x !== null && x !== undefined &&
                y !== NaN && y !== null && y !== undefined) {
                vertexList.push(new Vertex2D(x, y));
            }
        }

        return vertexList;
    }

    /*static getVertexListCenter(vertexList: Vertex2D[]): Common.NullableNumber {
        const vertexCount = vertexList.length;

        if (vertexCount === 0)
            return null;

        if (vertexCount < 2)
            return 0.0;

        const halfLen = Geometry.getVertexListLength(vertexList) / 2;
        let len = 0.0;
        let vPrev = vertexList[0];

        for (let i = 1; i < vertexCount; i++) {
            const vCurrent = vertexList[i];
            len += vPrev.getDistance(vCurrent);

            if (len >= halfLen) {
                return vCurrent.getLinearVertex(vPrev, len - halfLen);
            }

            vPrev = vCurrent;
        }

        return null;
    }*/

    static getVertexListLength(vertexList: Vertex2D[]): number {
        let len = 0.0;
        const vertexCount = vertexList.length;

        if (vertexCount < 2) {
            return len;
        }

        let vPrev = vertexList[0];

        for (let i = 1; i < vertexCount; i++) {
            const vCurrent = vertexList[i];
            len += vPrev.getDistance(vCurrent);
            vPrev = vCurrent;
        }

        return len;
    }

    static getPolylineDistance(vertexList: Vertex2D[], vertex: Vertex2D): Common.NullableNumber {
        let len = null;
        const vertexCount = vertexList.length;

        if (vertexCount < 2) {
            return len;
        }

        let vPrev = vertexList[0];

        for (let i = 1; i < vertexCount; i++) {
            const vCurrent = vertexList[i];
            const length = Geometry.getLineDistance(vPrev, vCurrent, vertex);

            if (length === 0.0) {
                return 0.0;
            }
            else if (len === null || len > length) {
                len = length;
            }

            vPrev = vCurrent;
        }

        return len;
    }

    static getLineDistance(v1: Vertex2D, v2: Vertex2D, vertex: Vertex2D): number {
        const a = vertex.getDistance(v1);
        const b = v1.getDistance(v2);
        const c = vertex.getDistance(v2);

        if (a <= 0.001 || c <= 0.001) {
            return 0.0;
        }
        if (b <= 0.001) {
            return a;
        }

        const dCos = (a * a + b * b - c * c) / 2 / a / b;
        const _vertex = v1.getLinearVertex(v2, dCos * a);
        const dLen = _vertex.getDistance(vertex);

        const dAngle1 = Geometry.getAngle(vertex, v1, v2);
        const dAngle2 = Geometry.getAngle(vertex, v2, v1);
        const halfPI = Math.PI / 2;

        if (dAngle1 <= halfPI && dAngle2 <= halfPI) {
            return dLen;
        }

        return a > c ? c : a;
    }

    static getAngle(v1: Vertex2D, vCenter: Vertex2D, v2: Vertex2D): number {
        const a = v1.getDistance(vCenter);
        const b = v2.getDistance(vCenter);
        const c = v1.getDistance(v2);

        let cosData = (a * a + b * b - c * c) / 2 / a / b;

        if (cosData < -1.0)
            cosData = -1.0;
        else if (cosData > 1.0)
            cosData = 1.0;

        return Math.acos(cosData);
    }

    static getAngle3(x1: number, y1: number, z1: number, xCenter: number, yCenter: number, zCenter: number, x3: number, y3: number, z3: number): number {
        const a = Geometry.getDistance3(x1, y1, z1, xCenter, yCenter, zCenter);
        const b = Geometry.getDistance3(x3, y3, z3, xCenter, yCenter, zCenter);
        const c = Geometry.getDistance3(x1, y1, z1, x3, y3, z3);

        let cosData = (a * a + b * b - c * c) / 2 / a / b;

        if (cosData < -1.0)
            cosData = -1.0;
        else if (cosData > 1.0)
            cosData = 1.0;

        return Math.acos(cosData);
    }

    static getDistance3(x1: number, y1: number, z1: number, x2: number, y2: number, z2: number): number {
        return Math.sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
    }

    static getDistance4(x1: number, y1: number, z1: number, w1: number, x2: number, y2: number, z2: number, w2: number): number {
        return Math.sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2) + (w1 - w2) * (w1 - w2));
    }

    // begin에서 target을 향해 len만큼 이동한 지점의 버텍스를 얻어온다.
    static getLinearVertex3(xBegin: number, yBegin: number, zBegin: number, xTarget: number, yTarget: number, zTarget: number, len: number): Common.Vector3Array {
        const distance = Geometry.getDistance3(xBegin, yBegin, zBegin, xTarget, yTarget, zTarget);

        if (distance <= 0.001)
            return [xBegin, yBegin, zBegin];

        const x = xBegin + (xTarget - xBegin) * len / distance;
        const y = yBegin + (yTarget - yBegin) * len / distance;
        const z = zBegin + (zTarget - zBegin) * len / distance;
        return [x, y, z];
    }

    // begin에서 target을 향해 len만큼 이동한 지점의 버텍스를 얻어온다.
    static getLinearVertex4(xBegin: number, yBegin: number, zBegin: number, wBegin: number, xTarget: number, yTarget: number, zTarget: number, wTarget: number, len: number): Common.Vector4Array {
        const distance = Geometry.getDistance4(xBegin, yBegin, zBegin, wBegin, xTarget, yTarget, zTarget, wTarget);

        if (distance <= 0.001)
            return [xBegin, yBegin, zBegin, wBegin];

        const x = xBegin + (xTarget - xBegin) * len / distance;
        const y = yBegin + (yTarget - yBegin) * len / distance;
        const z = zBegin + (zTarget - zBegin) * len / distance;
        const w = wBegin + (wTarget - wBegin) * len / distance;
        return [x, y, z, w];
    }

    // lineBegin과 lineEnd를 잇는 직선위에서 (x, y, z)와 가장 가까운 점을 알려준다.
    // noLimit : 무한히 긴 직선으로 간주하고 계산할 것인가?
    static getNearestVertex3(x: number, y: number, z: number, lineBeginX: number, lineBeginY: number, lineBeginZ: number, lineEndX: number, lineEndY: number, lineEndZ: number, noLimit: boolean): Common.Vector3Array {
        const len1 = Geometry.getDistance3(x, y, z, lineBeginX, lineBeginY, lineBeginZ);
        const len2 = Geometry.getDistance3(x, y, z, lineEndX, lineEndY, lineEndZ);

        if (len1 <= Geometry.Tolerance || len2 <= Geometry.Tolerance)
            return [x, y, z];

        const angle = Geometry.getAngle3(x, y, z, lineBeginX, lineBeginY, lineBeginZ, lineEndX, lineEndY, lineEndZ);
        const h = len1 * Math.cos(angle);

        const [_x, _y, _z] = Geometry.getLinearVertex3(lineBeginX, lineBeginY, lineBeginZ, lineEndX, lineEndY, lineEndZ, h);

        if (noLimit) {
            return [_x, _y, _z];
        }

        if (_x !== null && _y !== null && _z !== null) {
            if (Geometry.isIncludeInLine3(_x, _y, _z, lineBeginX, lineBeginY, lineBeginZ, lineEndX, lineEndY, lineEndZ)) {
                return [_x, _y, _z];
            }
        }

        return len1 < len2 ? [lineBeginX, lineBeginY, lineBeginZ] : [lineEndX, lineEndY, lineEndZ];
    }

    static isIncludeInLine3(x: number, y: number, z: number, lineBeginX: number, lineBeginY: number, lineBeginZ: number, lineEndX: number, lineEndY: number, lineEndZ: number): boolean {
        const len = Geometry.getDistanceFromLine3(x, y, z, lineBeginX, lineBeginY, lineBeginZ, lineEndX, lineEndY, lineEndZ, false);

        if (len <= Geometry.Tolerance)
            return true;

        return false;
    }

    // lineBegin과 lineEnd를 잇는 직선과 (x, y, z)와의 가장 가까운 거리를 알려준다.
    // noLimit : 무한히 긴 직선으로 간주하고 계산할 것인가?
    static getDistanceFromLine3(x: number, y: number, z: number, lineBeginX: number, lineBeginY: number, lineBeginZ: number, lineEndX: number, lineEndY: number, lineEndZ: number, noLimit: boolean): number {
        const a = Geometry.getDistance3(x, y, z, lineBeginX, lineBeginY, lineBeginZ);
        const b = Geometry.getDistance3(lineBeginX, lineBeginY, lineBeginZ, lineEndX, lineEndY, lineEndZ);
        const c = Geometry.getDistance3(x, y, z, lineEndX, lineEndY, lineEndZ);

        if (a <= Geometry.Tolerance || c <= Geometry.Tolerance)
            return 0.0;
        if (b <= Geometry.Tolerance)
            return a;

        const dCos = (a * a + b * b - c * c) / 2 / a / b;
        const [_x, _y, _z] = Geometry.getLinearVertex3(lineBeginX, lineBeginY, lineBeginZ, lineEndX, lineEndY, lineEndZ, dCos * a);

        if (_x !== null && _y !== null && _z !== null) {
            const len = Geometry.getDistance3(_x, _y, _z, x, y, z);

            if (noLimit) {
                return len;
            }

            const angle1 = Geometry.getAngle3(x, y, z, lineBeginX, lineBeginY, lineBeginZ, lineEndX, lineEndY, lineEndZ);
            const angle2 = Geometry.getAngle3(x, y, z, lineEndX, lineEndY, lineEndZ, lineBeginX, lineBeginY, lineBeginZ);

            const halfPI = Math.PI / 2;

            if (angle1 <= halfPI && angle2 <= halfPI)
                return len;
        }

        return a > c ? c : a;
    }
}