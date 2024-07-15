1. 데이터베이스 리버스 엔지니어링
    
    -- MySQL
	dotnet ef dbcontext scaffold "Server=192.168.1.205;Port=3306;Database=kdms;User=root;Password=20wellsdb19!@;" Pomelo.EntityFrameworkCore.MySql --context-dir Infrastructure/Reverse --output-dir Infrastructure/Reverse/Models
    

    -- MSSQL
	dotnet ef dbcontext scaffold "Data Source=192.168.1.205,3306;Database=kdms_server;User Id=sa;Password=20wellsdb19!@;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer --context-dir Infrastructure/Reverse --output-dir Infrastructure/Reverse/Models
    dotnet ef dbcontext scaffold "Data Source=127.0.0.1,51433;Database=k_h2ems_server;User Id=sa;Password=choshin01)!;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer --context-dir Infrastructure/Reverse --output-dir Infrastructure/Reverse/Models


    protected readonly IConfiguration _configuration;

    public KdmsContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
