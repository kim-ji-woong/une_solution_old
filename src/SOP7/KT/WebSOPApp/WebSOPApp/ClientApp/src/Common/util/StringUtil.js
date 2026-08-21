export default class StringUtil {
    static getDoubleString(num/*: number*/) {
        if (num < 10) {
            return "0" + num;
        }

        return num;
    }
}