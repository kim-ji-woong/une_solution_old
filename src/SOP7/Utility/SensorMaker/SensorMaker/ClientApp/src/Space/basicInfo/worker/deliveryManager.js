//table row -> treeNode Drag&Drop
export class DeliveryManager {
    constructor(obj) {
        this.object = obj;
        this.initDragDrop = false;
        this.element = null;
        this.fileID = 0;
        this.main = null;
    }

    initDragEvents(element) {
        if (element) {
            this.element = element;
            this.initDragDrop = true;
            element.addEventListener("dragenter", this.handleDragIn);
            element.addEventListener("dragleave", this.handleDragOut);
            element.addEventListener("dragover", this.handleDragOver);
            element.addEventListener("drop", this.handleDrop);
        }
    }

    resetDragEvents(element) {
        if (element) {
            element.removeEventListener("dragenter", this.handleDragIn);
            element.removeEventListener("dragleave", this.handleDragOut);
            element.removeEventListener("dragover", this.handleDragOver);
            element.removeEventListener("drop", this.handleDrop);
        }
    }

    onChangeEquipZone = (e/*: ChangeEvent<HTMLInputElement> | any*/) => {        
        if (this.object.props.parentFrm.selectedRows && this.object.props.parentFrm.selectedRows.length > 0) {
            this.object.onChangeSensorZone(this.object.props.parentFrm.selectedRows);
        }
    }

    handleDragIn = (e/*: DragEvent*/) => {        
        e.preventDefault();
        e.stopPropagation(); 

        if (this.object) {
            if (!this.object.state.isSelectedNode) {
                this.object.setState({ isSelectedNode: true });
            }
        }
    }

    handleDragOut = (e/*: DragEvent*/) => {
        e.preventDefault();
        e.stopPropagation();

        if (this.object) {
            if (this.object.state.isSelectedNode) {
                this.object.setState({ isSelectedNode: false });
            }
        }
    }

    handleDragOver = (e/*: DragEvent*/) => {
        e.preventDefault();
        e.stopPropagation();

        //if (e.dataTransfer?.files) {
        //    this.setDragging(true);
        //}
    }

    handleDrop = (e/*: DragEvent*/) => {
        e.preventDefault();
        e.stopPropagation();
        
        this.onChangeEquipZone(e);
        this.setDragging(false);

        if (this.object) {
            if (this.object.state.isSelectedNode) {
                this.object.setState({ isSelectedNode: false });
            }
        }
    }

    setDragging(isDragging) {
        if (this.object) {
            this.object.setState({ isDragging });
        }
    }
}