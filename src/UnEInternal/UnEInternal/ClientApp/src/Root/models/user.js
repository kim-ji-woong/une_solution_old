export class User {
    constructor(name, level) {
        // 이름
        this.name = name;
        // 직급
        this.level = level;
        // 남은 휴가일수
        this.userDays = 0;
        // 전체 휴가일수
        this.totalDays = 0;
    }
}
