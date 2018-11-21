import { FilesModel } from "./files-model.interface";

export interface UploadFileResponse {
  message: string;
  filesData: FilesModel[];
}
