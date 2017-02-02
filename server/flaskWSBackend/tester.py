import flaskWSBackend as api

def run_tests():
    json_result = api.add_tag('tag1')
    result = json.loads(json_result)
    print(result)