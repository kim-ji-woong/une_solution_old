import React, { Component } from 'react';
import '../Root/css/custom.css';
import { VacationMenus } from './vacationMenus';
import { VacationContents } from './vacationContents';
import { VacationController } from '../Root/services/vacationController';
import { Layout } from '../Root/layout';

import { AccountController } from '../Root/services/accountController';

export class VacationBody extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            selectedMenu: VacationMenus.MyVacations,
            loading: true,
            loadingMessage: "사용자 정보를 얻어오고 있습니다.",
            history: null,
            managerRequest: null,
            membersHistory: null,
            holidays: null,
            errors: null
        };
    }

    componentDidMount() {
        this.getUserHistory();
    }

    async getUserHistory() {
        if (!this.props.loginUser) {
            return;
        }

        const date = new Date();
        const holidays = await this.getHolidays(date.getFullYear());
        const result = await VacationController.requestHistory(this.props.loginUser, date.getFullYear(), date.getMonth() + 1, date.getDate());

        if (result?.success) {
            if (this.props.loginUser?.isTeamLeader) {
                const managerRequestData = await VacationController.requestManagerData(this.props.loginUser, date.getFullYear());

                if (managerRequestData?.success) {
                    const membersHistory = await VacationController.requestMemberHistory(this.props.loginUser.userID);

                    if (membersHistory?.success) {
                        this.setState({
                            selectedMenu: this.state.selectedMenu,
                            loading: false,
                            loadingMessage: "",
                            history: result,
                            managerRequest: managerRequestData,
                            membersHistory: membersHistory,
                            holidays,
                            errors: null
                        });
                    }
                    else {
                        this.setState({
                            selectedMenu: this.state.selectedMenu,
                            loading: false,
                            loadingMessage: "",
                            history: result,
                            managerRequest: managerRequestData,
                            membersHistory: null,
                            holidays,
                            errors: null
                        });
                    }
                }
                else {
                    this.setState({
                        selectedMenu: this.state.selectedMenu,
                        loading: false,
                        loadingMessage: "",
                        history: result,
                        managerRequest: null,
                        membersHistory: null,
                        holidays,
                        errors: null
                    });
                }
            }
            else {
                const membersHistory = await VacationController.requestMemberHistory(this.props.loginUser.userID);

                if (membersHistory?.success) {
                    this.setState({
                        selectedMenu: this.state.selectedMenu,
                        loading: false,
                        loadingMessage: "",
                        history: result,
                        managerRequest: null,
                        membersHistory: membersHistory,
                        holidays,
                        errors: null
                    });
                }
                else {
                    this.setState({
                        selectedMenu: this.state.selectedMenu,
                        loading: false,
                        loadingMessage: "",
                        history: result,
                        managerRequest: null,
                        membersHistory: null,
                        holidays,
                        errors: null
                    });
                }
            }
        }
        else {
            this.setState({
                selectedMenu: this.state.selectedMenu,
                loading: false,
                loadingMessage: "",
                history: null,
                managerRequest: null,
                membersHistory: null,
                holidays,
                errors: result.message
            });
        }
    }

    async getHolidays(year) {
        const [thisSuccess, thisMessage, thisHolidays] = await VacationController.requestHolidays(year);
        const [lastSuccess, lastMessage, lastHolidays] = await VacationController.requestHolidays(year - 1);
        const [nextSuccess, nextMessage, nextHolidays] = await VacationController.requestHolidays(year + 1);

        const holidays = [];

        if (lastSuccess) {
            for (const holiday of lastHolidays) {
                holidays.push(holiday);
            }
        }

        if (thisSuccess) {
            for (const holiday of thisHolidays) {
                holidays.push(holiday);
            }
        }

        if (nextSuccess) {
            for (const holiday of nextHolidays) {
                holidays.push(holiday);
            }
        }

        return holidays;
    }

    updateHistory = (history, historyNextYear) => {
        const loginUser = this.props.loginUser;

        if (loginUser) {
            this.state.membersHistory.memberHistories[loginUser.id] = history;

            if (historyNextYear) {
                this.state.membersHistory.memberHistoriesNextYear[loginUser.id] = historyNextYear;
            }
        }

        this.setState({ history });
    }

    getNextYearHistory = (loginUser) => {
        if (loginUser) {
            const memberHistoriesNextYear = this.state.membersHistory.memberHistoriesNextYear;

            if (memberHistoriesNextYear) {
                return memberHistoriesNextYear[loginUser.id];
            }
        }

        return null;
    }

    getLastYearHistory = (loginUser) => {
        if (loginUser) {
            const memberHistoriesLastYear = this.state.membersHistory.memberHistoriesLastYear;

            if (memberHistoriesLastYear) {
                return memberHistoriesLastYear[loginUser.id];
            }
        }

        return null;
    }

    onSelectMenu = (menu) => {
        if (this.state.selectedMenu !== menu) {
            this.setState({ selectedMenu: menu });
        }
    }

    onRemoveRequest = (requestID, isNormal) => {
        this.getUserHistory();
        /*const managerRequest = { ...this.state.managerRequest };

        if (isNormal) {
            // 일반휴가
            const count = managerRequest.waitingRequests.length;

            for (let i = 0; i < count; i++) {
                const wait = managerRequest.waitingRequests[i];

                if (wait.requestID === requestID) {
                    managerRequest.waitingRequests.splice(i, 1);
                    break;
                }
            }
        }
        else {
            // 특별휴가
            const count = managerRequest.waitingRequestSpecialVacations.length;

            for (let i = 0; i < count; i++) {
                const wait = managerRequest.waitingRequestSpecialVacations[i];

                if (wait.requestID === requestID) {
                    managerRequest.waitingRequestSpecialVacations.splice(i, 1);
                    break;
                }
            }
        }

        this.setState({
            managerRequest: managerRequest
        });*/
    }

    addVacationHistory = (vacation) => {
        this.getUserHistory();
        /*if (this.state.history) {
            this.state.history.usedVacations.push(vacation);
        }

        this.setState({ selectedMenu: VacationMenus.MyVacations, history: this.state.history });*/
    }

    render() {
        if (this.state.loading) {
            return (
                <div className="bodyArea">
                    <h2>{this.state.loadingMessage}</h2>
                </div>
            );
        }
        else if (this.state.errors) {
            return (
                <div className="bodyArea">
                    <h2>{this.state.errors}</h2>
                </div>
            );
        }

        return (
            <>
                <div className="bodyArea">
                    <VacationMenus loginUser={this.props.loginUser} options={this.props.options} onLogin={this.props.onLogin} onLogout={this.props.onLogout} managerRequest={this.state.managerRequest} onSelectMenu={this.onSelectMenu} selectedMenu={this.state.selectedMenu} />
                    <VacationContents loginUser={this.props.loginUser} holidays={this.state.holidays} options={this.props.options} history={this.state.history} managerRequest={this.state.managerRequest} membersHistory={this.state.membersHistory} onSelectMenu={this.onSelectMenu} selectedMenu={this.state.selectedMenu} addVacationHistory={this.addVacationHistory} removeRequest={this.onRemoveRequest} updateHistory={this.updateHistory} getNextYearHistory={this.getNextYearHistory} getLastYearHistory={this.getLastYearHistory}/>
               </div>
            </>
        );
    }
}
