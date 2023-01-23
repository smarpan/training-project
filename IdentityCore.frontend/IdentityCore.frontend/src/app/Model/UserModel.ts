import { ClaimModel } from "./claimModel";

export interface UserModel {
  id:string,
  userName: string,
  email: string,
  phoneNumber: string,
  gender: string,
  profileImage:string,
  role: any,
  claim?:ClaimModel[]
}
