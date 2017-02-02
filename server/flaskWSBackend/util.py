from flask import jsonify

def get_insert_response(type, value):
    result = {}
    result["Type"] = type
    result["value"] = value
    response = jsonify(result)
    return response

def insert_rejected(cause):
    return get_insert_response("InsertRejected",cause)

def insert_succeeded(value):
	return get_insert_response("LastInsertId", value)
	