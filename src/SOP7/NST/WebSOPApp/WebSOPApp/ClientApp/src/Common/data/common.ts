export type NullableNumber = number | null;
export type NullableString = string | null;
export type NullableObject = object | null;

export type Vector3 = {
    x: number,
    y: number,
    z: number
}

export type Vector4 = {
    x: number,
    y: number,
    z: number,
    w: number
}

export type Vector3Array = [NullableNumber, NullableNumber, NullableNumber];
export type Vector4Array = [NullableNumber, NullableNumber, NullableNumber, NullableNumber];