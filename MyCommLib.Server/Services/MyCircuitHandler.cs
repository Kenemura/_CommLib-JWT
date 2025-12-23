namespace MyCommLib.Server.Services;

using MyCommLib.Shared.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

// ** Comment by Ken **
// This CircutHandler set the user info in the IState service
public class MyCircuitHandler : CircuitHandler, IDisposable
{
    private readonly AuthenticationStateProvider _asp;
    private readonly IState _state;

    public MyCircuitHandler(AuthenticationStateProvider asp, IState state)
    {
        _asp = asp;
        _state = state;
    }
    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken token)
    {
        _asp.AuthenticationStateChanged += AuthChanged;
        return base.OnCircuitOpenedAsync(circuit, token);
    }
    private void AuthChanged(Task<AuthenticationState> task)
    {
        _ = UpdateAuth(task);
        async Task UpdateAuth(Task<AuthenticationState> task)
        {
            try
            {
                var auth = await task;
                _state.SetUser(auth.User);
            }
            catch { }
        }
    }
    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken token)
    {
        var auth = await _asp.GetAuthenticationStateAsync();
        _state.SetUser(auth.User);
    }
    public void Dispose()
    {
        _asp.AuthenticationStateChanged -= AuthChanged;
    }
}
