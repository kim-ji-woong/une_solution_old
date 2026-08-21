import React, { Component } from 'react';
import styles from '../css/toolbar.module.css';
import modelStyles from '../css/_3d.module.css';
import $ from 'jquery';

class Toolbar extends Component {
    constructor(props) {
        super(props);
    }


    componentDidMount() {

        $(document).ready(function () {
            var button = $('.' + modelStyles.moveMe);
            var bar = $('.' + modelStyles.progressBar);
            var barWidth = bar.outerWidth();
            var clickPosition;
            var percentage = 0;
            var buttonPosition;
            $(window).resize(function () {
                barWidth = bar.outerWidth();
                setButton();
            });
            var setButton = function () {
                buttonPosition = percentage * barWidth - 10;
                button.css("width", buttonPosition + 'px');
            };
            $('.' + modelStyles.progressBar).click(function (e) {
                clickPosition = e.pageX - $(this).offset().left;
                percentage = clickPosition / barWidth;
                setButton();
                $('.' + modelStyles.percentage).text(Math.round(percentage * 100) + "%");
            });
        });


        $(document).ready(function () {
            var button = $('.' + modelStyles.moveMe2);
            var bar = $('.' + modelStyles.progressBar2);
            var barWidth = bar.outerWidth();
            var clickPosition;
            var percentage = 0;
            var buttonPosition;
            $(window).resize(function () {
                barWidth = bar.outerWidth();
                setButton();
            });
            var setButton = function () {
                buttonPosition = percentage * barWidth - 10;
                button.css("width", buttonPosition + 'px');
            };
            $('.' + modelStyles.progressBar2).click(function (e) {
                clickPosition = e.pageX - $(this).offset().left;
                percentage = clickPosition / barWidth;
                setButton();
                $('.' + modelStyles.percentage2).text(Math.round(percentage * 100) + "%");
            });
        });

       $(document).ready(function () {
            $('.' + styles.lightSet).hover(function () {
                $('.' + modelStyles.lightBox).show();
            }, function () {
                $('.' + modelStyles.lightBox).hide();
            })
        });
    } 


    onClickNavigator = (event) => {
        const btn = event.target;

        if (btn.classList.contains(styles.on)) {
            btn.classList.remove(styles.on);
            $(btn).next().slideUp();
        }
        else {
            $(btn).next().slideDown();
            btn.classList.add(styles.on);
        }
    }

    getFloorElements() {
        if (this.props.buildingID === null || this.props.floorDatas === null || this.props.floorDatas.length === 0) {
            return <></>
        }

        const floorDatas = [ ...this.props.floorDatas ];

        return (
            <ul className={styles.dsnFloor}>
                {
                    floorDatas.map((floorData, index) => {
                        if (floorData.length === 0 || floorData[0] === null) {
                            return <></>
                        }

                        if (floorData.length <= 2) {
                            return <li key={"floor_" + index}><a onClick={() => this.props.moveToFloor(this.props.buildingID, floorData[0])}>{floorData[1]}</a></li>
                        }

                        // 현재층
                        return <li key={"floor_" + index}><a className={styles.on} onClick={() => this.props.moveToFloor(this.props.buildingID, floorData[0])}>{floorData[1]}</a></li>
                    })
                }
            </ul>
            );
    }

    render() {
        return (
            <div id={styles.dsNav}>
                <button onClick={this.onClickNavigator}>지도옵션 열기</button>
                <div>
                    <ul className={styles.dsnMenu}>
                        <li><a onClick={() => this.props.initViewport()}></a></li>
                        <li><a onClick={() => this.props.setInitialViewport()}> </a></li>
                        <li><a className={styles.lightSet}></a></li>
                    </ul>


                    {/* seekbar */}
                    <div className={modelStyles.lightBox}>
                        <span className={modelStyles.directLightText}>직사광</span>
                        <div className={modelStyles.lightwrapper}>
                            <div className={modelStyles.inner}>
                                <div className={modelStyles.percentage}>0%</div>
                                <div className={modelStyles.progressBar}>
                                    <div className={modelStyles.moveMe}>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <span className={modelStyles.dispersedLightText}>분산광</span>
                        <div className={modelStyles.lightwrapper2}>
                            <div className={modelStyles.inner2}>
                                <div className={modelStyles.percentage2}>0%</div>
                                <div className={modelStyles.progressBar2}>
                                    <div className={modelStyles.moveMe2}>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    {
                        this.getFloorElements()
                    }
                </div>
            </div>
        );
    }
}


export default Toolbar;