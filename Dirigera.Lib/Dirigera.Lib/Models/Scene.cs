namespace Dirigera.Lib.Models
{
    public class Scene
    {
        private readonly DirigeraManager _dirigeraManager;

        public string? Id { get; internal set; }
        public string? Name { get; internal set; }
        public string? Icon { get; internal set; }
        public string? Type { get; set; }
        public string? CreatedAt { get; set; }
        public string? LastCompleted { get; set; }
        public string? LastTriggered { get; set; }
        public int UndoAllowedDuration { get; set; }

        internal Scene(DirigeraManager dirigeraManager)
        {
            _dirigeraManager = dirigeraManager;
        }

        public async Task Trigger()
        {
            await _dirigeraManager.TriggerScene(this);
        }

        public async Task Undo()
        {
            await _dirigeraManager.UndoScene(this);
        }
    }
}
