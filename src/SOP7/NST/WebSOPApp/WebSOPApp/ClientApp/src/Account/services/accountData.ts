export class MessageResult {
    success: boolean = false;
    message: string = "";
}

export class ApplicationUser {
    id: number = -1;
    levelID: number = -1;
    level: string = "";
    userID: string = "";
    nickName: string = "";
    sessionKey: string = "";
}

export class LoginResult extends MessageResult {
    user: ApplicationUser | null = null;

    constructor() {
        super();
    }
}