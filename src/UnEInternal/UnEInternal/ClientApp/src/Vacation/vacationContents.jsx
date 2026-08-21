import React, { Component } from 'react';
import { VacationMenus } from './vacationMenus';
import styles from './css/vacation.module.css';
import { MyVacations } from './myVacations';
import { Request } from './request';
import { WaitResponse } from './waitResponse';
import { CompleteResponse } from './completeResponse';
import { MemberVacations } from './memberVacations';
import { TeamVacations } from './teamVacations';
import { RequestSpecialVacation } from './requestSpecialVacation';
import { CancelVacation } from './cancelVacation';

export class VacationContents extends Component {
    state = {
        year: null,
        month: null
    }

    componentDidMount() {
        const date = new Date();
        const year = date.getFullYear();
        const month = date.getMonth() + 1;
        this.setState({ year: year, month: month });
    }

    onChangeYear = (goNext) => {
        if (goNext) {
            this.setState({ year: this.state.year + 1, month: this.state.month });
        }
        else {
            this.setState({ year: this.state.year - 1, month: this.state.month });
        }
    }

    onChangeMonth = (goNext) => {
        let year = this.state.year;
        let month = 0;

        if (goNext) {
            month = this.state.month + 1;

            if (month > 12) {
                year++;
                month = 1;
            }
        }
        else {
            month = this.state.month - 1;

            if (month <= 0) {
                month = 12;
                year--;
            }
        }

        this.setState({ year: year, month: month });
    }

    getContents() {
        if (this.props.selectedMenu === VacationMenus.MyVacations) {
            return <MyVacations loginUser={this.props.loginUser} holidays={this.props.holidays} onLogin={this.props.onLogin} onLogout={this.props.onLogout} options={this.props.options} history={this.props.history} year={this.state.year} month={this.state.month} onChangeYear={this.onChangeYear} onChangeMonth={this.onChangeMonth} getNextYearHistory={this.props.getNextYearHistory} getLastYearHistory={this.props.getLastYearHistory} id={styles.aaaaa} />;
        }
        else if (this.props.selectedMenu === VacationMenus.Request) {
            return <Request loginUser={this.props.loginUser} holidays={this.props.holidays} onLogin={this.props.onLogin} onLogout={this.props.onLogout} options={this.props.options} history={this.props.history} year={this.state.year} month={this.state.month} onChangeYear={this.onChangeYear} onChangeMonth={this.onChangeMonth} addVacationHistory={this.props.addVacationHistory} getNextYearHistory={this.props.getNextYearHistory} getLastYearHistory={this.props.getLastYearHistory} />;
        }
        else if (this.props.selectedMenu === VacationMenus.WaitResponse) {
            return <WaitResponse loginUser={this.props.loginUser} onLogin={this.props.onLogin} onLogout={this.props.onLogout} options={this.props.options} managerRequest={this.props.managerRequest} removeRequest={this.props.removeRequest}/>;
        }
        else if (this.props.selectedMenu === VacationMenus.ResponseHistory) {
            return <CompleteResponse loginUser={this.props.loginUser} holidays={this.props.holidays} onLogin={this.props.onLogin} onLogout={this.props.onLogout} options={this.props.options} managerRequest={this.props.managerRequest} membersHistory={this.props.membersHistory} />;
        }
        else if (this.props.selectedMenu === VacationMenus.MemberHistory) {
            return <MemberVacations loginUser={this.props.loginUser} holidays={this.props.holidays} onLogin={this.props.onLogin} onLogout={this.props.onLogout} options={this.props.options} managerRequest={this.props.managerRequest} membersHistory={this.props.membersHistory} getNextYearHistory={this.props.getNextYearHistory} getLastYearHistory={this.props.getLastYearHistory} />;
        }
        else if (this.props.selectedMenu === VacationMenus.TeamHistory) {
            return <TeamVacations loginUser={this.props.loginUser} holidays={this.props.holidays} onLogin={this.props.onLogin} onLogout={this.props.onLogout} options={this.props.options} managerRequest={this.props.managerRequest} membersHistory={this.props.membersHistory} />;
        }
        else if (this.props.selectedMenu === VacationMenus.RequestSpecialHistory) {
            return <RequestSpecialVacation loginUser={this.props.loginUser} holidays={this.props.holidays} onLogin={this.props.onLogin} onLogout={this.props.onLogout} options={this.props.options} membersHistory={this.props.membersHistory} addVacationHistory={this.props.addVacationHistory} />;
        }
        else if (this.props.selectedMenu === VacationMenus.CancelHistory) {
            return <CancelVacation loginUser={this.props.loginUser} holidays={this.props.holidays} onLogin={this.props.onLogin} onLogout={this.props.onLogout} updateHistory={this.props.updateHistory} />;
        }

        return <></>;
    }

    getClassName() {
        if (this.props.selectedMenu === VacationMenus.WaitResponse ||
            this.props.selectedMenu === VacationMenus.ResponseHistory) {
            return styles.contentsArea + " " + styles.white;
        }

        return styles.contentsArea;
    }

    render() {
        const contents = this.getContents();

        return (
            <div className={this.getClassName()} >
                {contents}
            </div>
        );
    }
}