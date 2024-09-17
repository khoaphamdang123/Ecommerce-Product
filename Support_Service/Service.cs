using System.Security.Cryptography;
using System.Text;
using System.Management;
namespace Ecommerce_Product.Support_Serive;
public class Service
{
    private readonly ILogger<Service> _logger;
    public Service(ILogger<Service> logger)
    {
        this._logger=logger;
    }
     public string AddSha256(string data)
 {  
    StringBuilder sha_hash=new StringBuilder();
   try
 {
    using(SHA256 hash=SHA256.Create())
    {   
        byte[] bytes=hash.ComputeHash(Encoding.UTF8.GetBytes(data));

        for(int i=0;i<bytes.Length;i++)
        {
            sha_hash.Append(bytes[i].ToString("x2"));
        }
    }
 }
 catch(Exception er)
 {
    this._logger.LogTrace("AddSha256:"+er.Message);
 }
    return sha_hash.ToString();

 }
  public string GetCurrentFilePath(string file_name)
 {
    string full_file_path=Path.Combine(Directory.GetCurrentDirectory(),file_name);
    return full_file_path;
 }

public void writeCsvFile(string file_name,string data)
{
    try
    {
    string file_path=GetCurrentFilePath(file_name);
    using(var writer=new StreamWriter(file_path,true,Encoding.UTF8))
    {
        writer.WriteLine(data);
    }
    }
    catch(Exception ex)
    {
     this._logger.LogTrace("Write CSV Exception:"+ex.Message);
    }
}

public string generateRandomPassword()
{
  string random_password="Acb@";
  Random r= new Random();
  for(int i=1;i<=5;i++)
  {
    random_password+=r.Next(0,9);
  }
  return random_password;
}

public string getCurrentOs()
{
    var os_name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                      select x.GetPropertyValue("Caption")).FirstOrDefault();


return os_name != null ? os_name.ToString() : "Unknown";
}

}