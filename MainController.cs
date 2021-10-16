public class MainController : MonoBehavior{
    public delegate void KeyPressed(KeyCode key);
    public static KeyPressed keyPressed;

    public void Start(){

    }

    public void Update(){
        Event e = Event.current;
        if(e.isKey){
            if(keyPressed != null){
                keyPressed(e.keyCode);
            }
            
        }
    }
}