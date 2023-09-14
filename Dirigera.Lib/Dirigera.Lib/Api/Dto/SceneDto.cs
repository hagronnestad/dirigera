namespace Dirigera.Lib.Api.Dto
{
    internal class SceneDto
    {
        public string? Id { get; set; }
        public SceneInfoDto? Info { get; set; }
        public string? Type { get; set; }
        public string? CreatedAt { get; set; }
        public string? LastCompleted { get; set; }
        public string? LastTriggered { get; set; }
        public int UndoAllowedDuration { get; set; }
    }
}
