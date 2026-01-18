using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IniciarSesion : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContraseña;
    public Button botonLogin;
    public TextMeshProUGUI mensaje;

    [Header("Base de datos")]
    public string nombreDB = "MyDatabase.sqlite"; 
    private string rutaDB;

    [Header("Objetos a controlar")]
    public GameObject canvasLogin;   
    public GameObject canvasPrincipal; 

    void Start()
    {
        rutaDB = Application.persistentDataPath + "/" + nombreDB;

        botonLogin.onClick.AddListener(ComprobarLogin);
        mensaje.text = "Introduce usuario y contraseña";
    }

    void ComprobarLogin()
    {
        string usuario = inputUsuario.text.Trim();
        string contraseña = inputContraseña.text.Trim();

        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
        {
            mensaje.text = "Rellena todos los campos";
            return;
        }

        string dbUri = "URI=file:" + rutaDB;

        try
        {
            using (var conexion = new SqliteConnection(dbUri))
            {
                conexion.Open();

                string consulta = "SELECT * FROM Usuarios WHERE usuario=@usuario AND password=@password";

                using (var comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@usuario", usuario);
                    comando.Parameters.AddWithValue("@password", contraseña);

                    using (IDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            mensaje.text = "Inicio de sesión correcto";
                            Debug.Log("Usuario autenticado: " + usuario);

                            if (canvasPrincipal != null) canvasPrincipal.SetActive(true);
                            if (canvasLogin != null) canvasLogin.SetActive(false);
                        }
                        else
                        {
                            mensaje.text = "Usuario o contraseña incorrectos";
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            mensaje.text = "Error al conectar con la base de datos";
            Debug.LogError("Error SQLite: " + e.Message);
        }
    }
}
