export default class Vertex3D {
    x: number = 0;
    y: number = 0;
    z: number = 0;

    constructor(x: number, y: number, z: number) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    getDistance(vertex: Vertex3D): number {
        const w = this.x - vertex.x;
        const h = this.y - vertex.y;
        const t = this.z - vertex.z;

        return Math.sqrt(w * w + h * h + t * t);
    }

    getLinearVertex(target: Vertex3D, len: number): Vertex3D {
        const distance = this.getDistance(target);

        if (distance <= 0.001)
            return new Vertex3D(this.x, this.y, this.z);

        const x = this.x + (target.x - this.x) * len / distance;
        const y = this.y + (target.y - this.y) * len / distance;
        const z = this.z + (target.z - this.z) * len / distance;

        return new Vertex3D(x, y, z);
    }
}