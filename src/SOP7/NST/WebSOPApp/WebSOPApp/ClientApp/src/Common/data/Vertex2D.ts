export default class Vertex2D
{
    x: number = 0;
    y: number = 0;

    constructor(x: number, y: number)
    {
        this.x = x;
        this.y = y;
    }

    getDistance(vertex: Vertex2D): number {
        const w = this.x - vertex.x;
        const h = this.y - vertex.y;
        return Math.sqrt(w * w + h * h);
    }

    getLinearVertex(target: Vertex2D, len: number): Vertex2D {
        const distance = this.getDistance(target);

        if (distance <= 0.001)
            return new Vertex2D(this.x, this.y);

        const x = this.x + (target.x - this.x) * len / distance;
        const y = this.y + (target.y - this.y) * len / distance;
        return new Vertex2D(x, y);
    }
}