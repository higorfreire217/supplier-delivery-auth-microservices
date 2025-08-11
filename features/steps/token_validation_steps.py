import requests
import jwt
import time
from behave import given, when, then

API_URL = "http://localhost:5104/Auth"
PROTECTED_ENDPOINT_URL = "http://localhost:5104/Protected/resource"
REGISTER_API_URL = "http://localhost:5104/User/register"

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

    # Aguarde até expirar: considere que o token expira em 1 segundo
    # ou gere um token com expireHours=1 e aguarde 3600s (não prático para testes).
    # Normalmente expireHours não pode ser alterado no login, então
    # para simular, vamos decodificar e sobrescrever exp para o passado
    header = jwt.get_unverified_header(token)
    decoded = jwt.decode(token, options={"verify_signature": False, "verify_exp": False})
    decoded["exp"] = int(time.time()) - 10  # Expirado há 10 segundos
    
    # Gere token expirado (não assinado corretamente, mas suficiente para testar API que só valida exp)
    expired_token = jwt.encode(decoded, "minha-chave-jwt-super-secreta-123", algorithm=header["alg"])
    context.jwt_token = expired_token

@given('I have a malformed or tampered JWT token')
def step_impl(context):
    context.jwt_token = "malformed.jwt.token123456"

@given('I do not provide any JWT token')
def step_impl(context):
    context.jwt_token = None

@when('I access a protected endpoint')
def step_impl(context):
    headers = {}
    if context.jwt_token:
        headers["Authorization"] = f"Bearer {context.jwt_token}"
    context.response = requests.get(PROTECTED_ENDPOINT_URL, headers=headers)

@then('my request is authorized')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.status_code == 200

@then('I receive the requested resource')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    # Depende da resposta esperada, aqui apenas valida que há dados
    assert context.response.text

@then('my request is denied')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.status_code in [401, 403]

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
    assert "expired" in msg or "expirado" in msg

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
    assert "invalid" in msg or "inválido" in msg

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
    assert "required" in msg or "necessária" in msg or "requerida" in msg