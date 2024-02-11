export interface IHttpResult<T> {
    StatusCode: number;
    Message: string;
    Result: T;
}

export interface ILoginResult {
    token: string;
    expiration: string;
    user: IUser;
    refreshToken: any;
}

export interface IUser {
    id: string;
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    profilePicture: string;
    roles: string[];
}

export interface IRegisterUser {
    firstName: string;
    lastName: string;
    userName: string;
    email: string;
    dob: Date;
    profilePicture: any;
    role: number;
    password: string;
}

export interface IUserUpdate {
    firstName: string;
    lastName: string;
    userName: string;
    email: string;
    profileImage: string;
    dob: string;
}

export interface ILoginContext {
    password: string;
    userName: string;
    rememberMe?: boolean;
}

export interface IRefreshTokenContext {
    token: string;
}

export interface IChangePasswordContext{
    currentPassword: string;
    newPassword: string;
}
