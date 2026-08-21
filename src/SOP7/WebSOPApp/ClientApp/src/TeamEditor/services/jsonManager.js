export class JsonManager {
    static makeRequestTemporaryMembers() {
        const json = {
            "requestTemporaryMembers": true
        }

        return JSON.stringify(json);
    }
}