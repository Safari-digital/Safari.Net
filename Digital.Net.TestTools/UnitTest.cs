using Digital.Net.Core.Environment;

namespace Digital.Net.TestTools;

public abstract class UnitTest
{
    protected UnitTest()
    {
        AspNetEnv.Set(AspNetEnv.Test);
    }
}
