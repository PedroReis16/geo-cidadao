namespace GeoCidadao.Models.Enums
{
    public enum ErrorCodes
    {
        UNKNOWN_ERROR = 1,
        VALIDATION_ERROR = 2,
        ENTITY_NOT_FOUND = 3,
        USERNAME_REQUIRED = 4,
        EMAIL_REQUIRED = 5,
        NAME_REQUIRED = 6,
        USER_NOT_FOUND = 7,
        INVALID_MEDIA = 8,
        FILE_HASH_REQUIRED = 9,
        FILE_EXTENSION_REQUIRED = 10,
        USER_ID_REQUIRED = 11,
        CONTENT_REQUIRED = 12,
        POST_MEDIA_LIMIT_EXCEEDED = 13,
        POST_NOT_FOUND = 14,
    }
}