import requests
import jwt
import time
from behave import given, when, then

API_URL = "http://localhost:5104/Auth"
REGISTER_API_URL = "http://localhost:5104/User/register"
VALIDATE_TOKEN_URL = f"{API_URL}/validate"

@given('I have a valid JWT token')
def step_impl(context):
    email = "validtoken@example.com"
    password = "SenhaValida123"
    payload = {
        "name": "Test User",
        "email": email,
        "password": password
    }
    requests.post(REGISTER_API_URL, json=payload)
    login_response = requests.post(f"{API_URL}/login", json={
        "email": email,
        "password": password
    })
    assert login_response.status_code == 200
    data = login_response.json()
    context.jwt_token = data["jwtToken"]

@given('I have an expired JWT token')
def step_impl(context):
    email = "expiredtoken@example.com"
    password = "SenhaValida123"
    payload = {
        "name": "Expired User",
        "email": email,
        "password": password
    }
    requests.post(REGISTER_API_URL, json=payload)
    login_response = requests.post(f"{API_URL}/login", json={
        "email": email,
        "password": password
    })
    assert login_response.status_code == 200
    data = login_response.json()
    token = data["jwtToken"]

    header = jwt.get_unverified_header(token)
    decoded = jwt.decode(token, options={"verify_signature": False, "verify_exp": False})
    decoded["exp"] = int(time.time()) - 10  # Expirado há 10 segundos
    # TODO: Para produção, o backend deve validar a assinatura.
    expired_token = jwt.encode(decoded, "b7f8c2e1a9d4f6e3b2c1a8d7e6f5c4b3a1d2e3f4c5b6a7d8e9f0b1c2d3e4f5g6", algorithm=header["alg"])
    context.jwt_token = expired_token

@given('I have a malformed or tampered JWT token')
def step_impl(context):
    context.jwt_token = "malformed.jwt.token123456"

@given('I do not provide any JWT token')
def step_impl(context):
    context.jwt_token = None

@when('I access a protected endpoint')
def step_impl(context):
    # TODO: Quando o microserviço Supplier estiver implementado, este step deve ser extendido para acessar um endpoint protegido real.
    headers = {}
    json_data = {}
    if context.jwt_token:
        json_data = { "jwtToken": context.jwt_token }
    context.response = requests.post(VALIDATE_TOKEN_URL, json=json_data, headers=headers)

@then('my request is authorized')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.status_code == 200
    data = context.response.json()
    assert data.get("isValid") is True
    # TODO: No futuro, validar acesso a uma rota protegida.

@then('I receive the requested resource')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.text
    # TODO: No futuro, validar conteúdo da resposta de uma rota protegida.

@then('my request is denied')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    # Aceita 401 ou resposta com isValid=False
    if context.response.status_code in [401, 403]:
        assert True
    else:
        data = context.response.json()
        assert data.get("isValid") is False
    # TODO: No futuro, validar resposta HTTP de uma rota protegida.

@then('I receive an error message indicating the token has expired')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    data = {}
    try:
        data = context.response.json()
    except Exception:
        pass
    msg = (data.get("errorMessage") or context.response.text).lower()
    assert "expirado" in msg or "expired" in msg

@then('I receive an error message indicating the token is invalid')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    data = {}
    try:
        data = context.response.json()
    except Exception:
        pass
    msg = (data.get("errorMessage") or context.response.text).lower()
    assert "inválido" in msg or "invalid" in msg

@then('I receive an error message indicating authentication is required')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    data = {}
    try:
        data = context.response.json()
    except Exception:
        pass
    msg = (data.get("errorMessage") or context.response.text).lower()
    assert (
        "necessária" in msg or
        "requerida" in msg or
        "required" in msg or
        "inválido" in msg or
        "invalid" in msg or
        "expirado" in msg or
        "expired" in msg
    )
    # TODO: No futuro, validar mensagem de erro de uma rota protegida.