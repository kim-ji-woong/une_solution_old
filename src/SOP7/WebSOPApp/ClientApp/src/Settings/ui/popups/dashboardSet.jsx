import React, { Component } from 'react';
import { Container } from 'reactstrap';
import DatePicker from 'react-datepicker';
import { ko } from 'date-fns/esm/locale';
import $ from 'jquery';

import newStyles from '../../../Common/css/newStyle.module.css';
import newDefaults from '../../../Common/css/newDefault.module.css';
import 'react-datepicker/dist/react-datepicker.css';

class DashboardSet extends Component {
    constructor(props) {
		super(props);

		this.state = {
			dashboardBegin: null,
			dashboardEnd: null,
        }

		this.props = props;
		this.state.dashboardBegin = new Date();
		this.state.dashboardEnd = new Date();
	}

	componentDidMount() {
		this.initData();
	}

	initData() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let dashboardBegin = this.props.settings.dashboardBegin;
		let dashboardEnd = this.props.settings.dashboardEnd;

		if (dashboardBegin === null || dashboardBegin === undefined || dashboardBegin === "today")
			dashboardBegin = new Date();
		else
			dashboardBegin = new Date(dashboardBegin);

		if (dashboardEnd === null || dashboardEnd === undefined || dashboardEnd === "today")
			dashboardEnd = new Date();
		else
			dashboardEnd = new Date(dashboardEnd);

		this.setState({ dashboardBegin: dashboardBegin, dashboardEnd: dashboardEnd});
	}

	onChangeDashboardBegin = (date) => {
		this.setState({ dashboardBegin: date });
		$("input:radio[name='stgDate']").prop('checked', false);

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let korFormat = year + "-" + month + "-" + day;

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.dashboardBegin = korFormat;
	}

	onChangeDashboardEnd = (date) => {
		this.setState({ dashboardEnd: date });
		$("input:radio[name='stgDate']").prop('checked', false);

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let korFormat = year + "-" + month + "-" + day;

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.dashboardEnd = korFormat;
	}

	onClickToday = () => {
		let date = new Date();

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let korFormat = year + "-" + month + "-" + day;

		this.setState({ dashboardBegin: date, dashboardEnd: date });

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.dashboardBegin = korFormat;
		this.props.settings.dashboardEnd = korFormat;
	}

	onClickDefault = () => {
		let date = new Date();

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let korFormat = year + "-" + month + "-" + day;

		this.setState({ dashboardBegin: date, dashboardEnd: date });
		$("input:radio[name='stgDate']").prop('checked', false);

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.dashboardBegin = "today";
		this.props.settings.dashboardEnd = "today";
    }

	onClickWeek = () => {
		let today = new Date();

		let date = new Date();
		date.setDate(date.getDate() - 7);

		this.setState({ dashboardBegin: date, dashboardEnd: today });

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let dateValue = year + "-" + month + "-" + day;

		year = today.getFullYear();
		month = today.getMonth() + 1;
		day = today.getDate();

		let todayValue = year + "-" + month + "-" + day;

		this.props.settings.dashboardBegin = dateValue;
		this.props.settings.dashboardEnd = todayValue;
	}

	onClickMonth = () => {
		let today = new Date();

		let date = new Date();
		date.setMonth(date.getMonth() - 1);

		this.setState({ dashboardBegin: date, dashboardEnd: today });

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let dateValue = year + "-" + month + "-" + day;

		year = today.getFullYear();
		month = today.getMonth() + 1;
		day = today.getDate();

		let todayValue = year + "-" + month + "-" + day;

		this.props.settings.dashboardBegin = dateValue;
		this.props.settings.dashboardEnd = todayValue;
    }

	render() {

        return (
            <>
				<ul className={newStyles.stgTab + " " + newStyles.single}>
					<li><a href="#" className={newStyles.on}>일반</a></li>
				</ul> 
				<div className={newStyles.stgList}>
					<div className={newStyles.stgName}>
						<h5>표출 데이터 기간 설정</h5>
						<span className={newStyles.stgTltp} data-tooltip="표출 데이터 기간을 설정 합니다."></span>
						<ul className={newStyles.stgProd}>
							<li>
								<div className={newStyles.datepicker}>
									<DatePicker name="datepicker01" id="datepicker01" className={newStyles.dsrTxt}
										dateFormat="yyyy-MM-dd"
										locale={ko}
										maxDate={new Date()}
										selected={this.state.dashboardBegin}
										onChange={date => this.onChangeDashboardBegin(date)} />
								</div>
							</li>
							<li>~</li>
							<li>
								<div className={newStyles.datepicker}>
									<DatePicker name="datepicker02" id="datepicker02" className={newStyles.dsrTxt}
										dateFormat="yyyy-MM-dd"
										locale={ko}
										maxDate={new Date()}
										selected={this.state.dashboardEnd}
										onChange={date => this.onChangeDashboardEnd(date)} />
								</div>
							</li>
							<li><a onClick={this.onClickDefault} >기본값으로 변경</a></li>
							<li><input type="radio" name="stgDate" id="stgDate01" onClick={this.onClickToday} /><label for="stgDate01">오늘</label></li>
							<li><input type="radio" name="stgDate" id="stgDate02" onClick={this.onClickWeek} /><label for="stgDate02">1주</label></li>
							<li><input type="radio" name="stgDate" id="stgDate03" onClick={this.onClickMonth} /><label for="stgDate03">1개월</label></li>
						</ul>
					</div>
				</div>
            </>
        );
    }
}

export default DashboardSet;