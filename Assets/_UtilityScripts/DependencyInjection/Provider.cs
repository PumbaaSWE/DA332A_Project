using UnityEngine;


/**
 * This is example of usage
 * 
 */
public class Provider : MonoBehaviour, IDependencyProvider {

    [Inject]
    IDependencyProvider genericProvider; //any inteface works

    [Inject]
    void GetService(ServiceA serviceA)
    {
        //this.serviceA = serviceA;
    }


    [Provide]
    public ServiceA ProvideA()
    {
        return new ServiceA();
    }

    [Provide]
    public IDependencyProvider ProvideProvider()
    {
        return this;
    }

}

public class ServiceA
{
    public void Init()
    {
        Debug.Log("ServiceA Init() called");
    }
}

