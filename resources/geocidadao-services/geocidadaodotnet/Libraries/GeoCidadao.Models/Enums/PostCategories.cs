namespace GeoCidadao.Models.Enums
{
    public enum PostCategory
    {
        // ---------------------------
        // Segurança pública
        // ---------------------------
        CRIME = 1,
        VANDALISM = 2,
        PUBLIC_DISORDER = 3,
        ILLEGAL_OCCUPATION = 4,

        // ---------------------------
        // Trânsito e mobilidade
        // ---------------------------
        TRAFFIC_ACCIDENT = 10,
        DAMAGED_SIGNAGE = 11,
        MISSING_SIGNAGE = 12,
        TRAFFIC_CONGESTION = 13,
        ILLEGAL_PARKING = 14,
        LACK_OF_ACCESSIBILITY = 15,

        // ---------------------------
        // Infraestrutura urbana
        // ---------------------------
        ROAD_HOLE = 20,
        PUBLIC_LIGHTING = 21,
        DAMAGED_PAVEMENT = 22,
        CLOGGED_DRAIN = 23,
        DAMAGED_BRIDGE = 24,
        DAMAGED_SIDEWALK = 25,
        ILLEGAL_CONSTRUCTION = 26,

        // ---------------------------
        // Meio ambiente
        // ---------------------------
        ACCUMULATED_GARBAGE = 30,
        IRREGULAR_WASTE_DISPOSAL = 31,
        POLLUTION = 32,
        FIRE = 33,
        DEFORESTATION = 34,
        FLOODING = 35,

        // ---------------------------
        // Serviços públicos
        // ---------------------------
        WATER_OUTAGE = 40,
        POWER_OUTAGE = 41,
        GARBAGE_COLLECTION_FAILURE = 42,

        // ---------------------------
        // Saúde e bem-estar
        // ---------------------------
        ABANDONED_ANIMAL = 50,
        AGGRESSIVE_ANIMAL = 51,
        LACK_OF_SANITATION = 52,
        POOR_PUBLIC_SPACE_MAINTENANCE = 53,

        // ---------------------------
        // Condições sociais
        // ---------------------------
        LACK_OF_SOCIAL_PROGRAMS = 60,
        ILLEGAL_SETTLEMENT = 61,
        HOMELESS_PERSON = 62,

        // ---------------------------
        // Eventos e aglomerações
        // ---------------------------
        EVENT_SHOW = 70,
        EVENT_PROTEST = 71,
        EVENT_FAIR = 72,
        TEMPORARY_INTERDICTION = 73,
        LARGE_GATHERING = 74,

        // ---------------------------
        // Reclamações
        // ---------------------------
        LACK_OF_PUBLIC_RESPONSE = 80,
        INEFFICIENT_INSPECTION = 81,
        ABANDONED_PUBLIC_EQUIPMENT = 82
    }
}