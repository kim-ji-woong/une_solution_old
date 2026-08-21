import { JsonManager } from './jsonManager';

export class VacationController {
    static async requestManager(user, days) {
        try {
            const jsonData = JsonManager.makeRequestManager(user, days);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestSpecialVacationManager(user) {
        try {
            const jsonData = JsonManager.makeRequestSpecialVacationManager(user);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestHistory(user, year, month, day) {
        try {
            const jsonData = JsonManager.makeRequestHistory(user, year, month, day);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestVacation(user, days, description) {
        try {
            const jsonData = JsonManager.makeRequestVacation(user, days, description);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestSpecialVacation(manager, members, days, reason) {
        try {
            const userIDs = [];
            const memberCount = members.length;

            for (let i = 0; i < memberCount; i++) {
                const member = members[i];
                userIDs.push(member.userID);
            }

            const jsonData = JsonManager.makeRequestSpecialVacation(manager, userIDs, days, reason);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestManagerData(user, year) {
        try {
            const jsonData = JsonManager.makeRequestManagerData(user, year);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async processRequest(requestID, permit, managerUserID, managerDescription) {
        try {
            const jsonData = JsonManager.makeProcessRequest(requestID, permit, managerUserID, managerDescription, true);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async processSpecialVacationRequest(requestID, permit, managerUserID, managerDescription) {
        try {
            const jsonData = JsonManager.makeProcessRequest(requestID, permit, managerUserID, managerDescription, false);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestMemberHistory(managerUserID) {
        try {
            const jsonData = JsonManager.makeRequestMemberHistory(managerUserID);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                JsonManager.setMemberHistory(result);
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestVacationList(userID, year) {
        try {
            const jsonData = JsonManager.makeRequestVacationList(userID, year);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return [result.success, result.message, result.vacations];
            }
        }
        catch (e) {
            console.log(e);
        }

        return [false, "데이터를 조회할 수 없습니다.", null];
    }

    static async requestCancelVacations(requestIDs) {
        try {
            const jsonData = JsonManager.makeRequestCancelVacations(requestIDs);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return [result.success, result.message, result.history, result.historyNextYear];
            }
        }
        catch (e) {
            console.log(e);
        }

        return [false, "휴가취소가 실패하였습니다.", null, null];
    }

    static async requestHolidays(year, month = null) {
        try {
            const jsonData = JsonManager.makeRequestHolidays(year, month);

            const res = await fetch('/api/Vacation', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return [result.success, result.message, result.holidays];
            }
        }
        catch (e) {
            console.log(e);
        }

        return [false, "공휴일 정보를 얻어오는데 실패하였습니다.", null];
    }
}