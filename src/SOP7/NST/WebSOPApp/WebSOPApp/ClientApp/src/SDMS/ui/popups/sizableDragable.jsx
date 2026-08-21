import React, { Component } from 'react';
import styles from '../../css/sdms.module.css';

/*export type PopupState = {
    id: number,
    width: string,
    height: string,
    x: string,
    y: string
};

export interface Props {
    popupType: string,
    popupState: PopupState | null | undefined,
    setActiveDragPopup: (popupType: string) => void,
    setPopupState: (popupState: PopupState) => void
}

export interface State {
    dragOffsetX: number,
    dragOffsetY: number,
    maxScreenWidth: number,
    maxScreenHeight: number,
    resizeType: string,
    originalX: number,
    originalY: number,
    originalWidth: number,
    originalHeight: number,
    originalMouseX: number,
    originalMouseY: number,
    popupMinWidth: number,
    popupMinHeight: number,
    preMousePosition: {
        x: number,
        y: number
    },
    popup: HTMLElement
}*/

export class Panel {
    // 팝업 마우스 드래그 이벤트 리스너
    //private popupDragMouseMove: (event: MouseEvent) => void;
    //팝업 리사이즈 이벤트 리스너
    //private popupResizeMouseMove: (event: MouseEvent) => void;

    // 팝업 마우스 드래그 이벤트 리스너
    /*protected*/ setPopupDragMouseMove(state/*: State*/) {
        this.popupDragMouseMove = (event/*: MouseEvent*/) => {
            let mousePosition = {
                x: event.clientX,
                y: event.clientY
            }

            //움직여야할 좌표
            let moveX = mousePosition.x + state.dragOffsetX;
            let perMoveX = ((moveX / state.maxScreenWidth) * 100);

            let moveY = mousePosition.y + state.dragOffsetY;
            let perMoveY = ((moveY / state.maxScreenHeight) * 100);

            // 팝업 너비
            let width = state.popup.clientWidth;
            let left = state.popup.offsetLeft;

            // 팝업 높이
            let height = state.popup.clientHeight;
            let top = state.popup.offsetTop;

            let popupRightPos = width + left;   // 현재 위치에서 오른쪽 끝 절대 좌표
            let popupBottomPos = height + top;  // 현재 위치에서 아래쪽 끝 절대 좌표

            // 팝업이 화면밖으로 안나가도록 처리
            if (moveX > 0 && moveX + width < state.maxScreenWidth) {
                state.popup.style.left = perMoveX + '%';
            }
            else if (moveX + width > state.maxScreenWidth) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 끝지점이 우측 화면 밖을 벗어나게 될 때
                if (popupRightPos < state.maxScreenWidth) {
                    // 팝업을 우측 변에 고정
                    const lim = ((state.maxScreenWidth - width) / state.maxScreenWidth) * 100;
                    state.popup.style.left = lim + '%';
                }
                else if (state.preMousePosition.x > mousePosition.x) {
                    // 화면 오른쪽으로 팝업이 이미 벗어나 있을 때
                    state.popup.style.left = perMoveX + '%';
                }
            }
            else if (moveX <= 0) {
                // 드래그 도중 팝업 시작점이 좌측 화면 밖을 벗어나게 될 때
                if (left > 0) {
                    state.popup.style.left = '0%';
                }
                else if (state.preMousePosition.x < mousePosition.x) {
                    // 화면 왼쪽으로 팝업이 이미 벗어나 있을 때
                    state.popup.style.left = perMoveX + '%';
                }
            }

            if (moveY > 60 && moveY + height < state.maxScreenHeight) {
                state.popup.style.top = perMoveY + '%';
            }
            else if (moveY + height > state.maxScreenHeight) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 하단 끝지점이 화면 밖을 벗어나게 될 때
                if (popupBottomPos < state.maxScreenHeight) {
                    // 팝업을 아랫 변에 고정
                    const lim = ((state.maxScreenHeight - height) / state.maxScreenHeight) * 100;
                    state.popup.style.top = lim + '%';
                }
                else if (state.preMousePosition.y > mousePosition.y) {
                    // 화면 아래쪽으로 팝업이 이미 벗어나 있을 때
                    state.popup.style.top = perMoveY + '%';
                }
            }
            else if (moveY <= 60) {
                // 드래그 도중 상단 끝지점이 화면 밖을 벗어나게 될 때
                if (top > 60) {
                    // 팝업을 윗 변에 고정
                    // 상단 툴바는 항상 높이 60 고정이기 때문에 현재 화면 사이즈에서 60px의 비율을 계산한다.
                    const lim = (60 / state.maxScreenHeight) * 100;
                    state.popup.style.top = lim + '%';
                }
                else if (state.preMousePosition.y < mousePosition.y) {
                    // 화면 위쪽으로 팝업이 이미 벗어나 있을 때
                    state.popup.style.top = perMoveY + '%';
                }
            }
        }
    }

    //팝업 리사이즈 이벤트 리스너
    /*protected*/ setPopupResizeMouseMove(state/*: State*/) {
        this.popupResizeMouseMove = (event/*: MouseEvent*/) => {
            let sizeX = 0;
            let sizeY = 0;

            switch (state.resizeType) {
                // 수평
                case 'h-r': // 오른쪽 수평
                    sizeX = event.pageX - state.originalX;

                    if (event.pageX > 0 && event.pageX < state.maxScreenWidth && sizeX >= state.popupMinWidth) {
                        state.popup.style.width = sizeX + 'px';
                    }
                    break;
                case 'h-l': //왼쪽 수평
                    sizeX = state.originalWidth - (event.pageX - state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < state.maxScreenWidth && sizeX > state.popupMinWidth) {
                        state.popup.style.width = sizeX + 'px';

                        const pxLeft = (state.originalX + (event.pageX - state.originalMouseX));
                        state.popup.style.left = ((pxLeft / state.maxScreenWidth) * 100) + '%';
                    }
                    break;
                // 수직
                case 'v-b': // 바텀 수직
                    sizeY = event.pageY - state.originalY;

                    if (event.pageY > 60 && event.pageY < state.maxScreenHeight && sizeY > state.popupMinHeight) {
                        state.popup.style.height = sizeY + 'px';
                    }
                    break;
                case 'v-t': //탑 수직
                    sizeY = state.originalHeight - (event.pageY - state.originalMouseY);

                    if (event.pageY > 60 && event.pageY < state.maxScreenHeight && sizeY > state.popupMinHeight) {
                        state.popup.style.height = sizeY + 'px'

                        const pxTop = state.originalY + (event.pageY - state.originalMouseY);
                        state.popup.style.top = ((pxTop / state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                // 대각
                case 'd-rb': // 오른쪽 하단 대각
                    sizeX = event.pageX - state.originalX;
                    sizeY = event.pageY - state.originalY;

                    if (event.pageX > 0 && event.pageX < state.maxScreenWidth && sizeX > state.popupMinWidth) {
                        state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < state.maxScreenHeight && sizeY > state.popupMinHeight) {
                        state.popup.style.height = sizeY + 'px';
                    }
                    break;
                case 'd-rt': //오른쪽 상단 대각
                    sizeX = state.originalWidth + (event.pageX - state.originalMouseX);
                    sizeY = state.originalHeight - (event.pageY - state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < state.maxScreenWidth && sizeX > state.popupMinWidth) {
                        state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < state.maxScreenHeight && sizeY > state.popupMinHeight) {
                        state.popup.style.height = sizeY + 'px';

                        const pxTop = state.originalY + (event.pageY - state.originalMouseY);
                        state.popup.style.top = ((pxTop / state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                case 'd-lb': //왼쪽 하단 대각
                    sizeY = event.pageY - state.popup.getBoundingClientRect().top;
                    sizeX = state.originalWidth - (event.pageX - state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < state.maxScreenWidth && sizeX > state.popupMinWidth) {
                        state.popup.style.width = sizeX + 'px';

                        const pxLeft = (state.originalX + (event.pageX - state.originalMouseX));
                        state.popup.style.left = ((pxLeft / state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < state.maxScreenHeight && sizeY > state.popupMinHeight) {
                        state.popup.style.height = sizeY + 'px';
                    }
                    break;

                case 'd-lt': //왼쪽 상단 대각
                    sizeX = state.originalWidth - (event.pageX - state.originalMouseX);
                    sizeY = state.originalHeight - (event.pageY - state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < state.maxScreenWidth && sizeX > state.popupMinWidth) {
                        state.popup.style.width = sizeX + 'px';

                        const pxLeft = (state.originalX + (event.pageX - state.originalMouseX));
                        state.popup.style.left = ((pxLeft / state.maxScreenWidth) * 100) + '%';
                    }
                    if (event.pageY > 60 && event.pageY < state.maxScreenHeight && sizeY > state.popupMinHeight) {
                        state.popup.style.height = sizeY + 'px';

                        const pxTop = state.originalY + (event.pageY - state.originalMouseY);
                        state.popup.style.top = ((pxTop / state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                default:
            }
        }
    }

    // 팝업 드래그 시작(팝업을 누르고 있을 때)
    popupDragMousePress(event/*: MouseEvent*/, component/*: Component<Props, State>*/) {
        if (event.button == 0) {
            //마우스 조작중에 브라우저의 크기를 조절할 수 없으므로
            // 이 시점에 도큐먼트 전체 크기를 호출한다.
            component.setState({
                maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
                maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
                dragOffsetX: component.state.popup.offsetLeft - event.clientX,
                dragOffsetY: component.state.popup.offsetTop - event.clientY,
                preMousePosition: {
                    x: event.clientX,
                    y: event.clientY
                }
            });

            document.addEventListener('mousemove', this.popupDragMouseMove);
            document.addEventListener('mouseup', this.popupDragMouseUp);

            // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
            component.props.setActiveDragPopup(component.props.popupType);
        }
    }

    // 팝업 드래그 종료(mouse up)
    popupDragMouseUp = (component/*: Component<Props, State>*/) => {
        console.log('popup drag false')
        document.removeEventListener('mousemove', this.popupDragMouseMove);
        document.removeEventListener('mouseup', this.popupDragMouseUp);
        // 팝업 정보 DB 작성
        this.setPopupState(component);
    }

    // 팝업 리사이징(누르고 있을 때)
    popupResizeMousePress(event/*: MouseEvent*/, resizeType/*: string*/, component/*: Component<Props, State>*/) {
        /* resizeType
         * h-r      오른쪽 수평
         * h-l      왼쪽 수평
         * v-b      바텀 수직
         * v-t      탑 수직
         * d-rt     우측 상단 대각
         * d-rb     우측 하단 대각
         * d-lt     좌축 상단 대각
         * d-lb     좌측 하단 대각
        */
        component.setState({
            maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
            maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
            resizeType: resizeType,
            originalMouseX: event.pageX,
            originalMouseY: event.pageY,
            originalWidth: parseFloat(getComputedStyle(component.state.popup, null).getPropertyValue('width').replace('px', '')),
            originalHeight: parseFloat(getComputedStyle(component.state.popup, null).getPropertyValue('height').replace('px', '')),
            originalX: component.state.popup.getBoundingClientRect().left,
            originalY: component.state.popup.getBoundingClientRect().top,
        });

        this.component = component;
        document.addEventListener('mousemove', this.popupResizeMouseMove);

        document.addEventListener('mouseup', this.popupResizeMouseUp);
        // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
        component.props.setActiveDragPopup(component.props.popupType);
    }

    popupResizeMouseUp = () => {
        console.log('popup resize false');
        document.removeEventListener('mousemove', this.popupResizeMouseMove);
        document.removeEventListener('mouseup', this.popupResizeMouseUp);

        this.component.setState({ resizeType: null });
        this.setPopupState();
    }

    setPopupState(component/*: Component<Props, State>*/) {
        // 팝업 정보 DB 작성
        const perX = ((component.state.popup.offsetLeft / component.state.maxScreenWidth) * 100);
        const perY = ((component.state.popup.offsetTop) / component.state.maxScreenHeight * 100);
        const width = component.state.popup.offsetWidth;
        const height = component.state.popup.offsetHeight;

        //팝업 비활성화 될 때 컴포넌트가 사라져 계산식이 0으로 되는 현상이 발생함. 이때 DB 등록되는것을 방지
        if (perX > 0 && perY > 0 && width > 0 && height > 0) {
            const popupState = {
                id: component.props.popupState ? component.props.popupState.id : -1,
                x: perX + '%',
                y: perY + '%',
                height: height + 'px',
                width: width + 'px'
            }
            component.props.setPopupState(component.props.popupType, popupState);
        }
    }

    getResizeElements(component/*: Component<Props, State>*/) {
        return (
            <>
                <div className={styles.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r', component)} ></div>
                <div className={styles.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l', component)}></div>
                <div className={styles.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b', component)}></div>
                <div className={styles.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t', component)}></div>
                <div className={styles.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt', component)}></div>
                <div className={styles.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb', component)}></div>
                <div className={styles.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt', component)}></div>
                <div className={styles.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb', component)}></div>
            </>
            );
    }
}