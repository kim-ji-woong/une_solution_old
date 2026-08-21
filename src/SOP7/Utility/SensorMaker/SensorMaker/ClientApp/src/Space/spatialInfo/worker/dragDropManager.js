export class DragDropManager {
    constructor(obj) {
        this.object = obj;
        this.initDragDrop = false;
        this.fileID = 0;
    }

    initDragEvents(element) {
        if (element) {
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

    onChangeFiles = (e/*: ChangeEvent<HTMLInputElement> | any*/) => {
        let selectFiles/*: File[]*/ = [];
        let tempFiles/*: IFileTypes[]*/ = [];

        if (e.type === "drop") {
            selectFiles = e.dataTransfer.files;
        }
        else {
            selectFiles = e.target.files;
        }

        for (const file of selectFiles) {
            tempFiles = [
                ...tempFiles,
                {
                    id: this.fileID++,
                    object: file
                }
            ];
        }

        this.object.onDropFiles(tempFiles);
    }

    handleDragIn = (e/*: DragEvent*/) => {
        e.preventDefault();
        e.stopPropagation();
    }

    handleDragOut = (e/*: DragEvent*/) => {
        e.preventDefault();
        e.stopPropagation();

        this.setDragging(false);
    }

    handleDragOver = (e/*: DragEvent*/) => {
        e.preventDefault();
        e.stopPropagation();

        if (e.dataTransfer?.files) {
            this.setDragging(true);
        }
    }

    handleDrop = (e/*: DragEvent*/) => {
        e.preventDefault();
        e.stopPropagation();
        
        this.onChangeFiles(e);
        this.setDragging(false);
    }

    setDragging(isDragging) {
        this.object.setState({ isDragging });
    }
}