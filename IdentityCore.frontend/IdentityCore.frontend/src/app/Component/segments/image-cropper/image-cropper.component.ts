import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { NotificationService } from '../../../Service/notification.service';

@Component({
  selector: 'app-image-cropper',
  templateUrl: './image-cropper.component.html',
  styleUrls: ['./image-cropper.component.css']
})
export class ImageCropperComponent {

  croppedImage: any;
  containWithinAspectRatio = false;

  constructor(
    private dialogRef: MatDialogRef<ImageCropperComponent>,
    private notifyService: NotificationService,
    @Inject(MAT_DIALOG_DATA) public data: { rawImage: any,aspectRatio:number }) { }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
  }
  closeDialog() {
    this.dialogRef.close(this.croppedImage.replace(/^data:image\/[a-z]+;base64,/, ""));
  }

  toggleContainWithinAspectRatio() {
    this.containWithinAspectRatio = !this.containWithinAspectRatio;
  }

}
